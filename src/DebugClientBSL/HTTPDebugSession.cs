using com.github.yukon39.DebugBSL;
using com.github.yukon39.DebugBSL.debugger.debugAutoAttach;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugDBGUICommands;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugClientBSL
{
    public class HTTPDebugSession : IDebuggerClientSession, IDisposable
    {
        public readonly string InfobaseAlias;
        public readonly Guid DebugSession;
        private readonly HttpClientExecutor Executor;
        private bool Attached = false;
        private readonly Timer PingTimer;
        private readonly SemaphoreSlim PingSemaphore = new SemaphoreSlim(1, 1);

        public event TargetStartedHandler TargetStarted;
        public event TargetQuitHandler TargetQuit;
        public event MeasureProcessingHandler MeasureProcessing;

        private HTTPDebugSession(HttpClientExecutor executor, string infobaseAlias, Guid debugSession)
        {
            Executor = executor;
            InfobaseAlias = infobaseAlias;
            DebugSession = debugSession;  
            PingTimer = new Timer(async (e) => { await Loop(); });
        }

        public static HTTPDebugSession Create(HttpClientExecutor executor, string infobaseAlias) =>
            new HTTPDebugSession(executor, infobaseAlias, Guid.NewGuid());

        public static HTTPDebugSession Create(HttpClientExecutor executor, string infobaseAlias, Guid debugSession) => 
            new HTTPDebugSession(executor, infobaseAlias, debugSession);

        public bool IsAttached() => Attached;

        public async Task<AttachDebugUIResult> AttachAsync(char[] Password, DebuggerOptions Options)
        {
            var requestParameters = new RequestParameters("attachDebugUI");

            var request = new RDBGAttachDebugUIRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

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

            var request = new RDBGDetachDebugUIRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

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

            var request = new RDBGAttachDetachDebugTargetsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
                Attach = Attach,
            };
            request.ID.AddRange(targets);

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);
        }

        public async Task<List<DbgTargetStateInfo>> AttachedTargetsStatesAsync(string areaName)
        {
            var requestParameters = new RequestParameters("getDbgAllTargetStates");

            var request = new RDBGGetDbgAllTargetStatesRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
            };

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

            var request = new RDBGSetInitialDebugSettingsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
                Data = data
            };

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);

            //Logger.LogDebug("InitSettings successful");
        }

        public async Task SetAutoAttachSettingsAsync(DebugAutoAttachSettings autoAttachSettings)
        {
            var requestParameters = new RequestParameters("setAutoAttachSettings");

            var request = new RDBGSetAutoAttachSettingsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,

                AutoAttachSettings = autoAttachSettings
            };

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);

            //Logger.LogDebug("SetAutoAttachSettings successful");
        }

        public async Task ClearBreakOnNextStatementAsync()
        {
            var requestParameters = new RequestParameters("clearBreakOnNextStatement");

            var request = new RDBGClearBreakOnNextStatementRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

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
                DebugID = DebugSession
            };

            var request = new RDBGPingDebugUIRequest()
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            var response = await Executor.ExecuteAsync<RDBGPingDebugUIResponse>(request, requestParameters);
            var result = response.Result;

            //Logger.LogDebug("Ping result size is {size}", Result.Count);

            return result;
        }

        public async Task SetMeasureModeAsync(Guid measureMode)
        {
            var requestParameters = new RequestParameters("setMeasureMode");

            var request = new RDBGSetMeasureModeRequest()
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
                MeasureModeSeanceID = measureMode
            };

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
                    TargetStarted?.Invoke(StartEvent.TargetID);
                    break;

                case DBGUIExtCmdInfoQuit QuitEvent:
                    TargetQuit?.Invoke(QuitEvent.TargetID);
                    break;

                case DBGUIExtCmdInfoMeasure MeasureEvent:
                    MeasureProcessing?.Invoke(MeasureEvent.Measure);
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
