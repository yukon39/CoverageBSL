using com.github.yukon39.CoverageBSL.debugger;
using com.github.yukon39.CoverageBSL.debugger.debugAutoAttach;
using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using com.github.yukon39.CoverageBSL.debugger.debugDBGUICommands;
using com.github.yukon39.CoverageBSL.debugger.debugMeasure;
using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using com.github.yukon39.CoverageBSL.httpDebug;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL
{
    public class CoverageManager
    {
        private IDebuggerClient Client;
        private IDebuggerSession Session;

        private readonly List<DebugTargetType> TargetTypes = DefaultTargetTypes;
        private readonly List<string> AreaNames = new();

        public delegate void TargetStartedHandler(DebugTargetId TargetID);
        public event TargetStartedHandler TargetStarted;

        public delegate void TargetQuitHandler(DebugTargetId TargetID);
        public event TargetQuitHandler TargetQuit;

        public delegate void MeasureProcessingHandler(PerformanceInfoMain TargetID);
        public event MeasureProcessingHandler MeasureProcessing;

        public void Configure(Uri debuggerURI)
        {
            Client = HTTPDebugClient.Build(debuggerURI);
            Client.Test();
        }

        public void Attach(string infobaseAlias, char[] Password)
        {
            var Options = new DebuggerOptions();
            var Data = new HTTPServerInitialDebugSettingsData();

            var AutoAttachSettings = new DebugAutoAttachSettings();
            AutoAttachSettings.TargetType.AddRange(TargetTypes);
            AutoAttachSettings.AreaName.AddRange(AreaNames);

            Session = Client.CreateSession(infobaseAlias);

            Session.Attach(Password, Options);
            Session.InitSettings(Data);
            Session.ClearBreakOnNextStatement();
            Session.SetAutoAttachSettings(AutoAttachSettings);
        }

        public void Detach()
        {
            if (Session.IsAttached())
            {
                Session.Detach();
            }
        }

        private void Loop()
        {
            if (Session.IsAttached())
            {
                Session.Ping().ForEach(x => { InvokeEvent(x); });
            }
        }

        private static List<DebugTargetType> DefaultTargetTypes => new()
        {
            DebugTargetType.Client,
            DebugTargetType.ManagedClient,
            DebugTargetType.WEBClient,
            DebugTargetType.Server,
            DebugTargetType.ServerEmulation
        };

        private void InvokeEvent(DBGUIExtCmdInfoBase Command)
        {
            switch (Command)
            {
                case DBGUIExtCmdInfoStarted StartEvent:
                    Session.AttachDebugTarget(StartEvent.TargetID);
                    TargetStarted?.Invoke(StartEvent.TargetID);
                    break;

                case DBGUIExtCmdInfoQuit QuitEvent:
                    Session.DetachDebugTarget(QuitEvent.TargetID);
                    TargetQuit?.Invoke(QuitEvent.TargetID);
                    break;

                case DBGUIExtCmdInfoMeasure MeasureEvent:
                    MeasureProcessing?.Invoke(MeasureEvent.Measure);
                    break;
            }
        }
    }
}
