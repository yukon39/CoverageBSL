using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using com.github.yukon39.DebugBSL.Client.Data;
using com.github.yukon39.DebugBSL.Client.Internal;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugDBGUICommands;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;

namespace com.github.yukon39.DebugBSL.Client.Impl
{
    public class DebuggerClientSession : IDebuggerClientSession, IDisposable
    {
        private readonly SessionContext Context;
        private readonly DebuggerClientExecutor Executor;
        private readonly DebuggerClientTargetsManager TargetsManager;
        private readonly DebuggerClientMeasureManager MeasureManager;
        private volatile bool Attached = false;
        private bool Stopping = false;
        private bool Disposed = false;
        private readonly object StateSync = new object();
        private readonly Timer PingTimer;
        private readonly SemaphoreSlim PingSemaphore = new SemaphoreSlim(1, 1);

        public event TargetStartedHandler TargetStarted;
        public event TargetQuitHandler TargetQuit;
        public event MeasureProcessingHandler MeasureProcessing;

        private DebuggerClientSession(DebuggerClientExecutor executor, string infobaseAlias, Guid debugSession)
        {
            Executor = executor;
            Context = SessionContext.NewInstance(infobaseAlias, debugSession);
            PingTimer = new Timer(PingTimerCallback);

            TargetsManager = NewEntityManager<DebuggerClientTargetsManager>(Executor, Context);
            MeasureManager = NewEntityManager<DebuggerClientMeasureManager>(Executor, Context);
        }

        private T NewEntityManager<T>(DebuggerClientExecutor executor, SessionContext context) where T : DebuggerClientEntityManager
        {
            var manager = Activator.CreateInstance(typeof(T), new object[] { executor, context }) as T;
            manager.SubscribeSessionEvents(this);
            return manager;
        }

        public static DebuggerClientSession NewInstance(DebuggerClientExecutor executor, string infobaseAlias) =>
            new DebuggerClientSession(executor, infobaseAlias, Guid.NewGuid());

        public static DebuggerClientSession NewInstance(DebuggerClientExecutor executor, string infobaseAlias, Guid debugSession) =>
            new DebuggerClientSession(executor, infobaseAlias, debugSession);

        public bool IsAttached() => Attached;

        public IDebuggerClientTargets GetTargetsManager() => TargetsManager;

        public IDebuggerClientMeasure GetMeasureManager() => MeasureManager;

        public async Task<AttachDebugUIResult> AttachAsync(char[] Password, DebuggerOptions Options)
        {
            var requestParameters = new RequestParameters("attachDebugUI");

            var request = Context.NewSessionRequest<RDBGAttachDebugUIRequest>();

            //if (password.length > 0)
            //{
            //    var credentials = StringUtils.toByteArray(password);
            //    request.setCredentials(credentials);
            //}
            request.Options = Options;

            var response = await Executor.ExecuteAsync<RDBGAttachDebugUIResponse>(request, requestParameters);
            var result = response.Result;

            //Logger.LogDebug("Debug attach result is {result}", Result);

            lock (StateSync)
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException(nameof(DebuggerClientSession));
                }

                Attached = result.Equals(AttachDebugUIResult.Registered);

                if (Attached)
                {
                    Stopping = false;
                    StartTimer();
                }
            }

            return result;
        }

        public async Task<bool> DetachAsync()
        {
            var requestParameters = new RequestParameters("detachDebugUI");

            var request = Context.NewSessionRequest<RDBGDetachDebugUIRequest>();

            StopSession();
            await PingSemaphore.WaitAsync();
            try
            {
                var response = await Executor.ExecuteAsync<RDBGDetachDebugUIResponse>(request, requestParameters);
                var result = response.Result;

                //Logger.LogDebug("Debug detach result is {result}", Result);

                return result;
            }
            finally
            {
                PingSemaphore.Release();
            }
        }

        public async Task PingAsync()
        {
            await PingSemaphore.WaitAsync();
            try
            {
                await InvokeByPingAsync();
            }
            finally
            {
                PingSemaphore.Release();
            }
        }

        public async Task<bool> PingAsync(TimeSpan timeSpan)
        {
            var result = await PingSemaphore.WaitAsync(timeSpan);
            if (result)
            {
                try
                {
                    await InvokeByPingAsync();
                }
                finally
                {
                    PingSemaphore.Release();
                }
            }
            return result;
        }

        private async Task<List<DBGUIExtCmdInfoBase>> PingInternalAsync()
        {
            var requestParameters = new RequestParameters("pingDebugUIParams")
            {
                DebugID = Context.DebugSession
            };

            var request = Context.NewSessionRequest<RDBGPingDebugUIRequest>();

            var response = await Executor.ExecuteAsync<RDBGPingDebugUIResponse>(request, requestParameters);
            var result = response.Result;

            //Logger.LogDebug("Ping result size is {size}", Result.Count);

            return result;
        }

        private async Task Loop()
        {
            var result = await PingSemaphore.WaitAsync(TimeSpan.FromMilliseconds(500));
            if (!result)
            {
                return;
            }

            try
            {
                if (Attached)
                {
                    await InvokeByPingAsync();
                }
            }
            finally
            {
                PingSemaphore.Release();
            }
        }

        private async void PingTimerCallback(object state)
        {
            try
            {
                await Loop();
            }
            catch (Exception exception)
            {
                HandlePingTimerException(exception);
            }
        }

        private void HandlePingTimerException(Exception exception)
        {
            lock (StateSync)
            {
                if (Stopping || Disposed)
                {
                    return;
                }

                Stopping = true;
                Attached = false;
                StopTimer();
            }

            Trace.TraceError("Background debugger ping failed: {0}", exception);
        }

        private async Task InvokeByPingAsync() =>
            (await PingInternalAsync()).ForEach(x => { InvokeEvent(x); });

        private void InvokeEvent(DBGUIExtCmdInfoBase Command)
        {
            switch (Command)
            {
                case DBGUIExtCmdInfoStarted StartEvent:
                    TargetStarted?.Invoke(this, StartEvent.TargetID);
                    break;

                case DBGUIExtCmdInfoQuit QuitEvent:
                    TargetQuit?.Invoke(this, QuitEvent.TargetID);
                    break;

                case DBGUIExtCmdInfoMeasure MeasureEvent:
                    MeasureProcessing?.Invoke(this, MeasureEvent.Measure);
                    break;
            }
        }

        private void StartTimer() =>
            PingTimer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        private void StopTimer() =>
            PingTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        private void StopSession()
        {
            lock (StateSync)
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException(nameof(DebuggerClientSession));
                }

                Stopping = true;
                Attached = false;
                StopTimer();
            }
        }

        public void Dispose()
        {
            lock (StateSync)
            {
                if (Disposed)
                {
                    return;
                }

                Disposed = true;
                Stopping = true;
                Attached = false;
                StopTimer();
                PingTimer.Dispose();
            }

            PingSemaphore.Dispose();
        }
    }
}
