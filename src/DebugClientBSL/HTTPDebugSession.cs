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
        private readonly IDebuggerClient Client;
        private bool Attached = false;
        private Timer Timer;
        private readonly Mutex PingMutex = new Mutex();

        public event TargetStartedHandler TargetStarted;
        public event TargetQuitHandler TargetQuit;
        public event MeasureProcessingHandler MeasureProcessing;

        public HTTPDebugSession(HTTPDebugClient client, string infobaseAlias)
        {
            Client = client;
            InfobaseAlias = infobaseAlias;
            DebugSession = Guid.NewGuid();
        }

        public bool IsAttached() => Attached;

        public AttachDebugUIResult Attach(char[] Password, DebuggerOptions Options)
        {
            var requestParameters = new RequestParameters
            {
                Command = "attachDebugUI"
            };

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

            var response = Execute<RDBGAttachDebugUIResponse>(request, requestParameters);
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

        public bool Detach()
        {
            var RequestParameters = new RequestParameters
            {
                Command = "detachDebugUI"
            };


            var Request = new RDBGDetachDebugUIRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            lock (this)
            {
                var Response = Execute<RDBGDetachDebugUIResponse>(Request, RequestParameters);

                Attached = false;

                StopTimer();

            }

            //var Result = Response.Result;
            //Logger.LogDebug("Debug detach result is {result}", Result);

            return true;
        }

        public void AttachDebugTarget(DebugTargetIdLight target) => AttachDetachDebugTargets(new List<DebugTargetIdLight>() { target }, true);

        public void DetachDebugTarget(DebugTargetIdLight target) => AttachDetachDebugTargets(new List<DebugTargetIdLight>() { target }, false);

        private void AttachDetachDebugTargets(List<DebugTargetIdLight> targets, bool Attach)
        {
            var RequestParameters = new RequestParameters
            {
                Command = "attachDetachDbgTargets"
            };

            var Request = new RDBGAttachDetachDebugTargetsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
                Attach = Attach,
            };
            Request.ID.AddRange(targets);

            Execute<RDBGEmptyResponse>(Request, RequestParameters);
        }

        public List<DbgTargetStateInfo> AttachedTargetsStates(string areaName)
        {
            var requestParameters = new RequestParameters
            {
                Command = "getDbgAllTargetStates"
            };

            var request = new RDBGGetDbgAllTargetStatesRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
            };

            if (!string.IsNullOrEmpty(areaName))
            {
                request.DebugAreaName = areaName;
            }

            var response = Execute<RDBGGetDbgAllTargetStatesResponse>(request, requestParameters);
            var items = response.Item;

            return items;
        }

        public void InitSettings(HTTPServerInitialDebugSettingsData data)
        {
            var RequestParameters = new RequestParameters
            {
                Command = "initSettings"
            };


            var Request = new RDBGSetInitialDebugSettingsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
                Data = data
            };

            Execute<RDBGEmptyResponse>(Request, RequestParameters);

            //Logger.LogDebug("InitSettings successful");
        }

        public void SetAutoAttachSettings(DebugAutoAttachSettings autoAttachSettings)
        {
            var requestParameters = new RequestParameters
            {
                Command = "setAutoAttachSettings"
            };

            var request = new RDBGSetAutoAttachSettingsRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,

                AutoAttachSettings = autoAttachSettings
            };

            Execute<RDBGEmptyResponse>(request, requestParameters);

            //Logger.LogDebug("SetAutoAttachSettings successful");
        }

        public void ClearBreakOnNextStatement()
        {
            var RequestParameters = new RequestParameters
            {
                Command = "clearBreakOnNextStatement"
            };

            var Request = new RDBGClearBreakOnNextStatementRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            Execute<RDBGEmptyResponse>(Request, RequestParameters);

            //Logger.LogDebug("ClearBreakOnNextStatement successful");
        }

        public void Ping()
        {
            PingMutex.WaitOne();
            PingInternal().ForEach(x => { InvokeEvent(x); });
            PingMutex.ReleaseMutex();
        }

        private List<DBGUIExtCmdInfoBase> PingInternal()
        {
            var RequestParameters = new RequestParameters
            {
                Command = "pingDebugUIParams",
                DebugID = DebugSession
            };

            var Request = new RDBGPingDebugUIRequest()
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            var Response = Execute<RDBGPingDebugUIResponse>(Request, RequestParameters);
            var Result = Response.Result;

            //Logger.LogDebug("Ping result size is {size}", Result.Count);

            return Result;
        }

        public void SetMeasureMode(Guid measureMode)
        {
            var requestParameters = new RequestParameters
            {
                Command = "setMeasureMode",
            };

            var request = new RDBGSetMeasureModeRequest()
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias,
                MeasureModeSeanceID = measureMode
            };

            Execute<RDBGEmptyResponse>(request, requestParameters);
        }

        private void Loop()
        {
            if (Attached && PingMutex.WaitOne(TimeSpan.FromMilliseconds(500)))
            {
                PingInternal().ForEach(x => { InvokeEvent(x); });
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
            Timer = new Timer((e) => { Loop(); }, null, period, period);
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

        private async Task<T> ExecuteAsync<T>(IRDBGRequest request, IDebuggerClientRequestParameters parameters) where T : IRDBGResponse => 
            await Client.ExecuteAsync<T>(request, parameters).ConfigureAwait(false);

        private T Execute<T>(IRDBGRequest request, IDebuggerClientRequestParameters parameters) where T : IRDBGResponse
        {
            return ExecuteAsync<T>(request, parameters).GetAwaiter().GetResult();
        }
    }
}
