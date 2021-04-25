using com.github.yukon39.DebugBSL;
using com.github.yukon39.DebugBSL.debugger.debugAutoAttach;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using ScriptEngine;
using ScriptEngine.HostedScript.Library;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine.Values;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.Coverage
{
    [ContextClass(typeName: "CoverageSession", typeAlias: "СессияОтладки")]
    public class CoverageSession : AutoContext<CoverageSession>
    {
        private readonly IDebuggerClientSession DebuggerSession;
        private readonly List<DebugTargetType> TargetTypes = DefaultTargetTypes;
        private readonly List<string> AreaNames = new List<string>();
        private CoverageData coverageData;

        public CoverageSession(IDebuggerClient debuggerClient, string infobaseAlias)
        {
            DebuggerSession = debuggerClient.CreateSession(infobaseAlias);

            DebuggerSession.TargetStarted += HandlerTargetStarted;
            DebuggerSession.TargetQuit += HandlerTargetQuit;
            DebuggerSession.MeasureProcessing += HandlerMeasureProcessing;
        }

        [ContextMethod("Attach", "Подключить")]
        public void Attach(string password)
        {
            var attachResult = DebuggerSession.Attach(password.ToCharArray(), new DebuggerOptions());

            switch (attachResult)
            {
                case AttachDebugUIResult.Registered:
                    OnSuccessfulAttach();
                    break;

                case AttachDebugUIResult.CredentialsRequired:

                    throw new RuntimeException(
                        Locale.NStr("en = 'Credentials required';ru = 'Требуется указание пароля'"));

                case AttachDebugUIResult.NotRegistered:

                    throw new RuntimeException(
                        Locale.NStr("en = 'Not registered';ru = 'Не зарегистрирован'"));

                case AttachDebugUIResult.IBInDebug:

                    throw new RuntimeException(
                        Locale.NStr("en = 'IB already in debug mode';ru = 'База уже в режиме отладки'"));

                default:
                    throw new RuntimeException(
                        Locale.NStr("en = 'Unknown error';ru = 'Неизвестная ошибка'"));
            }
        }

        private void OnSuccessfulAttach()
        {
            var data = new HTTPServerInitialDebugSettingsData();

            var autoAttachSettings = new DebugAutoAttachSettings();
            autoAttachSettings.TargetType.AddRange(TargetTypes);
            autoAttachSettings.AreaName.AddRange(AreaNames);

            DebuggerSession.InitSettings(data);
            DebuggerSession.ClearBreakOnNextStatement();
            DebuggerSession.SetAutoAttachSettings(autoAttachSettings);
            DebuggerSession.AttachedTargetsStates("").ForEach(x => DebuggerSession.AttachDebugTarget(x.TargetID.TargetIdLight));
        }

        [ContextMethod("Detach", "Отключить")]
        public void Detach() => DebuggerSession.Detach();

        [ContextMethod("StartPerformanceMeasure", "НачатьЗамерПроизводительности")]
        public GuidWrapper StartPerformanceMeasure()
        {
            var measureId = Guid.NewGuid();
            DebuggerSession.SetMeasureMode(measureId);
            coverageData = new CoverageData();
            return new GuidWrapper(measureId.ToString());
        }

        [ContextMethod("StopPerformanceMeasure", "ЗавершитьЗамерПроизводительности")]
        public CoverageData StopPerformanceMeasure()
        {
            DebuggerSession.SetMeasureMode(Guid.Empty);
            return coverageData;
        }

        private void HandlerTargetStarted(DebugTargetId targetID)
        {
            DebuggerSession.AttachDebugTarget(targetID.TargetIdLight);
        }

        private void HandlerTargetQuit(DebugTargetId targetID)
        {
            DebuggerSession.DetachDebugTarget(targetID.TargetIdLight);
        }

        private void HandlerMeasureProcessing(PerformanceInfoMain performanceInfo)
        {
            lock (coverageData)
            {
                coverageData.TotalDurability += performanceInfo.TotalDurability;

                performanceInfo.ModuleData.ForEach(x => ProcessPerformanceInfoModule(x));
            }
        }

        private void ProcessPerformanceInfoModule(PerformanceInfoModule module)
        {
            var moduleBSL = new CoverageModuleId(module.ModuleID);
            var linesCoverage = (MapImpl)coverageData.Data.Retrieve(moduleBSL);
            if (linesCoverage == null)
            {
                linesCoverage = new MapImpl();
                coverageData.Data.Insert(moduleBSL, linesCoverage);
            }

            module.LineInfo.ForEach(x => ProcessPerformanceInfoLine(x, linesCoverage));
        }

        private void ProcessPerformanceInfoLine(PerformanceInfoLine line, MapImpl lineslinesCoverage)
        {
            lineslinesCoverage.Insert(NumberValue.Create(line.LineNo), BooleanValue.True);
        }

        private static List<DebugTargetType> DefaultTargetTypes => new List<DebugTargetType>()
        {
            DebugTargetType.Client,
            DebugTargetType.ManagedClient,
            DebugTargetType.WEBClient,
            DebugTargetType.Server,
            DebugTargetType.ServerEmulation
        };
    }
}
