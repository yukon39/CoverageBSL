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
    class HTTPDebugSession : IDebuggerClientSession, IDisposable
    {
        public readonly string InfobaseAlias;
        public readonly Guid DebugSession;
        private readonly Uri DebugServerURL;
        private bool Attached = false;
        private Timer Timer;
        private readonly Mutex PingMutex = new Mutex();

        public event TargetStartedHandler TargetStarted;
        public event TargetQuitHandler TargetQuit;
        public event MeasureProcessingHandler MeasureProcessing;

        public HTTPDebugSession(Uri debugServerURL, string infobaseAlias)
        {
            DebugServerURL = debugServerURL;
            InfobaseAlias = infobaseAlias;
            DebugSession = Guid.NewGuid();
        }

        public bool IsAttached() => Attached;

        public async Task<AttachDebugUIResult> AttachAsync(char[] Password, DebuggerOptions Options)
        {
            var requestParameters = new RequestParameters(DebugServerURL, "attachDebugUI");

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

            var response = await HttpClientExecutor.ExecuteAsync<RDBGAttachDebugUIResponse>(request, requestParameters);
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
            var requestParameters = new RequestParameters(DebugServerURL, "detachDebugUI");
            
            var request = new RDBGDetachDebugUIRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            // lock
            var response = await HttpClientExecutor.ExecuteAsync<RDBGDetachDebugUIResponse>(request, requestParameters);
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
            var requestParameters = new RequestParameters(DebugServerURL, "attachDetachDbgTargets");
            
            var request = new RDBGAttachDetachDebugTargetsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
                Attach = Attach,
            };
            request.ID.AddRange(targets);

            await HttpClientExecutor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);
        }

        public async Task<List<DbgTargetStateInfo>> AttachedTargetsStatesAsync(string areaName)
        {
            var requestParameters = new RequestParameters(DebugServerURL, "getDbgAllTargetStates");

            var request = new RDBGGetDbgAllTargetStatesRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
            };

            if (!string.IsNullOrEmpty(areaName))
            {
                request.DebugAreaName = areaName;
            }

            var response = await HttpClientExecutor.ExecuteAsync<RDBGGetDbgAllTargetStatesResponse>(request, requestParameters);
            var items = response.Item;

            return items;
        }

        public async Task InitSettingsAsync(HTTPServerInitialDebugSettingsData data)
        {
            var requestParameters = new RequestParameters(DebugServerURL, "initSettings");
           
            var request = new RDBGSetInitialDebugSettingsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
                Data = data
            };

            await HttpClientExecutor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);

            //Logger.LogDebug("InitSettings successful");
        }

        public async Task SetAutoAttachSettingsAsync(DebugAutoAttachSettings autoAttachSettings)
        {
            var requestParameters = new RequestParameters(DebugServerURL, "setAutoAttachSettings");
            
            var request = new RDBGSetAutoAttachSettingsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,

                AutoAttachSettings = autoAttachSettings
            };

            await HttpClientExecutor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);

            //Logger.LogDebug("SetAutoAttachSettings successful");
        }

        public async Task ClearBreakOnNextStatementAsync()
        {
            var requestParameters = new RequestParameters(DebugServerURL, "clearBreakOnNextStatement");
            
            var request = new RDBGClearBreakOnNextStatementRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            await HttpClientExecutor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);

            //Logger.LogDebug("ClearBreakOnNextStatement successful");
        }

        public async Task PingAsync()
        {
            PingMutex.WaitOne();
            (await PingInternalAsync()).ForEach(x => { InvokeEvent(x); });
            PingMutex.ReleaseMutex();
        }

        private async Task<List<DBGUIExtCmdInfoBase>> PingInternalAsync()
        {
            var requestParameters = new RequestParameters(DebugServerURL, "pingDebugUIParams")
            {
                DebugID = DebugSession
            };

            var request = new RDBGPingDebugUIRequest()
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            var response = await HttpClientExecutor.ExecuteAsync<RDBGPingDebugUIResponse>(request, requestParameters);
            var result = response.Result;

            //Logger.LogDebug("Ping result size is {size}", Result.Count);

            return result;
        }

        public async Task SetMeasureModeAsync(Guid measureMode)
        {
            var requestParameters = new RequestParameters(DebugServerURL, "setMeasureMode");
            
            var request = new RDBGSetMeasureModeRequest()
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
                MeasureModeSeanceID = measureMode
            };

            await HttpClientExecutor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);
        }

        private async Task Loop()
        {
            if (Attached && PingMutex.WaitOne(TimeSpan.FromMilliseconds(500)))
            {
                (await PingInternalAsync()).ForEach(x => { InvokeEvent(x); });
                PingMutex.ReleaseMutex();
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

        private void StartTimer()
        {
            StopTimer();

            var period = TimeSpan.FromSeconds(1);
            Timer = new Timer(async (e) => { await Loop(); }, null, period, period);
        }

        private void StopTimer()
        {
            if (Timer is Timer)
            {
                Timer.Dispose();
            }
            Timer = null;
        }

        public void Dispose()
        {
            StopTimer();
        }
    }
}
