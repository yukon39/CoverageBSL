using com.github.yukon39.DebugBSL.Client.Data;
using com.github.yukon39.DebugBSL.Client.Internal;
using com.github.yukon39.DebugBSL.debugger.debugAutoAttach;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugDBGUICommands;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client.Impl
{
    public class DebuggerClientSession : IDebuggerClientSession, IDisposable
    {
        private readonly SessionContext Context;
        private readonly DebuggerClientExecutor Executor;
        private bool Attached = false;
        private readonly Timer PingTimer;
        private readonly SemaphoreSlim PingSemaphore = new SemaphoreSlim(1, 1);

        public event TargetStartedHandler TargetStarted;
        public event TargetQuitHandler TargetQuit;
        public event MeasureProcessingHandler MeasureProcessing;

        private DebuggerClientSession(DebuggerClientExecutor executor, string infobaseAlias, Guid debugSession)
        {
            Executor = executor;
            Context = SessionContext.NewInstance(infobaseAlias, debugSession);
            PingTimer = new Timer(async (e) => { await Loop(); });
        }

        public static DebuggerClientSession NewInstance(DebuggerClientExecutor executor, string infobaseAlias) =>
            new DebuggerClientSession(executor, infobaseAlias, Guid.NewGuid());

        public static DebuggerClientSession NewInstance(DebuggerClientExecutor executor, string infobaseAlias, Guid debugSession) =>
            new DebuggerClientSession(executor, infobaseAlias, debugSession);

        public bool IsAttached() => Attached;

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

            lock (this)
            {
                Attached = result.Equals(AttachDebugUIResult.Registered);

                if (Attached)
                {
                    StartTimer();
                }
            }

            return result;
        }

        public async Task<bool> DetachAsync()
        {
            var requestParameters = new RequestParameters("detachDebugUI");

            var request = Context.NewSessionRequest<RDBGDetachDebugUIRequest>();

            // lock
            var response = await Executor.ExecuteAsync<RDBGDetachDebugUIResponse>(request, requestParameters);
            var result = response.Result;

            Attached = false;
            StopTimer();

            //Logger.LogDebug("Debug detach result is {result}", Result);

            return result;
        }

        public async Task AttachDebugTargetAsync(DebugTargetIdLight target) =>
            await AttachDetachDebugTargetsAsync(new List<DebugTargetIdLight>() { target }, true);

        public async Task DetachDebugTargetAsync(DebugTargetIdLight target) =>
            await AttachDetachDebugTargetsAsync(new List<DebugTargetIdLight>() { target }, false);

        private async Task AttachDetachDebugTargetsAsync(List<DebugTargetIdLight> targets, bool Attach)
        {
            var requestParameters = new RequestParameters("attachDetachDbgTargets");

            var request = Context.NewSessionRequest<RDBGAttachDetachDebugTargetsRequest>();
            request.Attach = Attach;
            request.ID.AddRange(targets);

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);
        }

        public async Task<List<DbgTargetStateInfo>> AttachedTargetsStatesAsync(string areaName)
        {
            var requestParameters = new RequestParameters("getDbgAllTargetStates");

            var request = Context.NewSessionRequest<RDBGGetDbgAllTargetStatesRequest>();

            if (!string.IsNullOrEmpty(areaName))
            {
                request.DebugAreaName = areaName;
            }

            var response = await Executor.ExecuteAsync<RDBGGetDbgAllTargetStatesResponse>(request, requestParameters);
            var items = response.Item;

            return items;
        }

        public async Task InitSettingsAsync(HTTPServerInitialDebugSettingsData data)
        {
            var requestParameters = new RequestParameters("initSettings");

            var request = Context.NewSessionRequest<RDBGSetInitialDebugSettingsRequest>();
            request.Data = data;

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);

            //Logger.LogDebug("InitSettings successful");
        }

        public async Task SetAutoAttachSettingsAsync(DebugAutoAttachSettings autoAttachSettings)
        {
            var requestParameters = new RequestParameters("setAutoAttachSettings");

            var request = Context.NewSessionRequest<RDBGSetAutoAttachSettingsRequest>();
            request.AutoAttachSettings = autoAttachSettings;

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);

            //Logger.LogDebug("SetAutoAttachSettings successful");
        }

        public async Task ClearBreakOnNextStatementAsync()
        {
            var requestParameters = new RequestParameters("clearBreakOnNextStatement");

            var request = Context.NewSessionRequest<RDBGClearBreakOnNextStatementRequest>();

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);

            //Logger.LogDebug("ClearBreakOnNextStatement successful");
        }

        public async Task PingAsync()
        {
            await PingSemaphore.WaitAsync();
            (await PingInternalAsync()).ForEach(x => { InvokeEvent(x); });
            PingSemaphore.Release();
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

        public async Task SetMeasureModeAsync(Guid measureModeSeanceID)
        {
            var requestParameters = new RequestParameters("setMeasureMode");

            var request = Context.NewSessionRequest<RDBGSetMeasureModeRequest>();
            request.MeasureModeSeanceID = measureModeSeanceID;

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);
        }

        private async Task Loop()
        {
            if (Attached && await PingSemaphore.WaitAsync(TimeSpan.FromMilliseconds(500)))
            {
                (await PingInternalAsync()).ForEach(x => { InvokeEvent(x); });
                PingSemaphore.Release();
            }
        }

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
            PingTimer.Change(TimeSpan.Zero, TimeSpan.Zero);

        public void Dispose()
        {
            PingTimer.Dispose();
            PingSemaphore.Dispose();
        }
    }
}
