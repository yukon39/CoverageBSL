using com.github.yukon39.CoverageBSL.debugger;
using com.github.yukon39.CoverageBSL.debugger.debugAutoAttach;
using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using com.github.yukon39.CoverageBSL.httpDebug;
using ScriptEngine.Machine.Contexts;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL
{
    [ContextClass(typeName: "МенеджерПокрытия", typeAlias: "CoverageManager")]
    public class CoverageManager : AutoContext<CoverageManager>
    {
        private IDebuggerClient Client;
        private IDebuggerSession Session;

        private readonly List<DebugTargetType> TargetTypes = DefaultTargetTypes;
        private readonly List<string> AreaNames = new();

        [ScriptConstructor(Name = "По умолчанию")]
        public static CoverageManager Constructor() => new();

        [ContextMethod("Настроить", "Configure")]
        public string Configure(string debuggerURI)
        {
            var DebuggetURI = new Uri(debuggerURI);
            Client = HTTPDebugClient.Build(DebuggetURI);
            return Client.ApiVersion();
        }

        [ContextMethod("Подключить", "Attach")]
        public AttachDebugUIResult Attach(string infobaseAlias, string Password)
        {
            var Options = new DebuggerOptions();
            var Data = new HTTPServerInitialDebugSettingsData();

            var AutoAttachSettings = new DebugAutoAttachSettings();
            AutoAttachSettings.TargetType.AddRange(TargetTypes);
            AutoAttachSettings.AreaName.AddRange(AreaNames);

            Session = Client.CreateSession(infobaseAlias);

            char[] EmptyPassword = Array.Empty<char>();

            var AttachResult = Session.Attach(EmptyPassword, Options);
            if (AttachResult == AttachDebugUIResult.Registered)
            {
                Session.InitSettings(Data);
                Session.ClearBreakOnNextStatement();
                Session.SetAutoAttachSettings(AutoAttachSettings);
                Session.AttachedTargetsStates(null).ForEach(x => Session.AttachDebugTarget(x.TargetID.TargetIdLight));
                Session.SetMeasureMode(Guid.NewGuid());
            }

            return AttachResult;
        }

        [ContextMethod("Отключить", "Detach")]
        public void Detach()
        {
            if (Session.IsAttached())
            {
                Session.Detach();
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
    }
}
