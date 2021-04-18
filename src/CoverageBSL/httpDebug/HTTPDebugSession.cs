using com.github.yukon39.CoverageBSL.debugger;
using com.github.yukon39.CoverageBSL.debugger.debugAutoAttach;
using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using com.github.yukon39.CoverageBSL.debugger.debugDBGUICommands;
using com.github.yukon39.CoverageBSL.debugger.debugMeasure;
using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Collections.Generic;
using System.Threading;

namespace com.github.yukon39.CoverageBSL.httpDebug
{
    class HTTPDebugSession : IDebuggerSession, IDisposable
    {
        public readonly string InfobaseAlias;
        public readonly Guid DebugSession;
        private readonly HTTPDebugClient Client;
        private bool Attached = false;
        private Timer Timer;

        public delegate void TargetStartedHandler(DebugTargetId TargetID);
        public event TargetStartedHandler TargetStarted;

        public delegate void TargetQuitHandler(DebugTargetId TargetID);
        public event TargetQuitHandler TargetQuit;

        public delegate void MeasureProcessingHandler(PerformanceInfoMain TargetID);
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
            var RequestParameters = new RequestParameters
            {
                Command = "attachDebugUI"
            };

            var Request = new RDBGAttachDebugUIRequest
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            //if (password.length > 0)
            //{
            //    var credentials = StringUtils.toByteArray(password);
            //    request.setCredentials(credentials);
            //}
            Request.Options = Options;

            var Response = Client.Execute<RDBGAttachDebugUIResponse>(Request, RequestParameters);
            var Result = Response.Result;

            //Logger.LogDebug("Debug attach result is {result}", Result);

            lock(this)
            {
                Attached = Result.Equals(AttachDebugUIResult.Registered);

                if (Attached)
                {
                    StartTimer();
                }
            }

            return Result;
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

            lock(this)
            {
                var Response = Client.Execute<RDBGDetachDebugUIResponse>(Request, RequestParameters);

                Attached = false;

                StopTimer();

            }

            //var Result = Response.Result;
            //Logger.LogDebug("Debug detach result is {result}", Result);

            return true;
        }

        public void AttachDebugTarget(DebugTargetIdLight target) => AttachDetachDebugTargets(new() { target }, true);

        public void DetachDebugTarget(DebugTargetIdLight target) => AttachDetachDebugTargets(new() { target }, false);

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

            Client.Execute<RDBGEmptyResponse>(Request, RequestParameters);
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

            var response = Client.Execute<RDBGGetDbgAllTargetStatesResponse>(request, requestParameters);
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

            Client.Execute<RDBGEmptyResponse>(Request, RequestParameters);

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

            Client.Execute<RDBGEmptyResponse>(request, requestParameters);

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

            Client.Execute<RDBGEmptyResponse>(Request, RequestParameters);

            //Logger.LogDebug("ClearBreakOnNextStatement successful");
        }

        public List<DBGUIExtCmdInfoBase> Ping()
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

            var Response = Client.Execute<RDBGPingDebugUIResponse>(Request, RequestParameters);
            var Result = Response.Result;

            //Logger.LogDebug("Ping result size is {size}", Result.Count);

            return Result;
        }

        public void SetMeasureMode(Guid measureMode)
        {
            var requestParameters = new RequestParameters
            {
                Command = "setMeasureMode",
                DebugID = DebugSession
            };

            var request = new RDBGSetMeasureModeRequest()
            {
                IdOfDebuggerUI = DebugSession,
                InfoBaseAlias = InfobaseAlias
            };

            if(measureMode != Guid.Empty)
            {
                request.MeasureModeSeanceID = measureMode;
            }

            Client.Execute<RDBGEmptyResponse>(request, requestParameters);
        }

        private void Loop()
        {
            lock(this) {
                if (Attached)
                {
                    Ping().ForEach(x => { InvokeEvent(x); });
                }
            }
        }

        private void InvokeEvent(DBGUIExtCmdInfoBase Command)
        {
            switch (Command)
            {
                case DBGUIExtCmdInfoStarted StartEvent:
                    AttachDebugTarget(StartEvent.TargetID.TargetIdLight);
                    TargetStarted?.Invoke(StartEvent.TargetID);
                    break;

                case DBGUIExtCmdInfoQuit QuitEvent:
                    DetachDebugTarget(QuitEvent.TargetID.TargetIdLight);
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
    }
}
