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
using System.Threading.Tasks;

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

            DebuggerSession.TargetStarted += HandlerTargetStartedAsync;
            DebuggerSession.TargetQuit += HandlerTargetQuitAsync;
            DebuggerSession.MeasureProcessing += HandlerMeasureProcessingAsync;
        }

        [ContextMethod("Attach", "Подключить")]
        public void Attach(string password) => 
            AttachConfigureAwait(password).GetAwaiter().GetResult();

        private async Task AttachConfigureAwait(string password) => 
            await AttachAsync(password).ConfigureAwait(false);

        private async Task AttachAsync(string password)
        {
            var attachResult = await DebuggerSession.AttachAsync(password.ToCharArray(), new DebuggerOptions());

            switch (attachResult)
            {
                case AttachDebugUIResult.Registered:
                    await OnSuccessfulAttachAsync();
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

        private async Task OnSuccessfulAttachAsync()
        {
            var data = new HTTPServerInitialDebugSettingsData();

            var autoAttachSettings = new DebugAutoAttachSettings();
            autoAttachSettings.TargetType.AddRange(TargetTypes);
            autoAttachSettings.AreaName.AddRange(AreaNames);

            await DebuggerSession.InitSettingsAsync(data);
            await DebuggerSession.ClearBreakOnNextStatementAsync();
            await DebuggerSession.SetAutoAttachSettingsAsync(autoAttachSettings);
            (await DebuggerSession.AttachedTargetsStatesAsync(""))
                .ForEach(async x => await DebuggerSession.AttachDebugTargetAsync(x.TargetID.TargetIdLight));
        }

        [ContextMethod("Detach", "Отключить")]
        public void Detach() => 
            DetachConfigureAwait().GetAwaiter().GetResult();

        private async Task DetachConfigureAwait() => 
            await DebuggerSession.DetachAsync().ConfigureAwait(false);

        [ContextMethod("StartPerformanceMeasure", "НачатьЗамерПроизводительности")]
        public GuidWrapper StartPerformanceMeasure() => 
            StartPerformanceMeasureConfigureAwait().GetAwaiter().GetResult();

        private async Task<GuidWrapper> StartPerformanceMeasureConfigureAwait() => 
            await StartPerformanceMeasureAsync().ConfigureAwait(false);

        private async Task<GuidWrapper> StartPerformanceMeasureAsync()
        {
            var measureId = Guid.NewGuid();
            await DebuggerSession.SetMeasureModeAsync(measureId);
            coverageData = new CoverageData();
            return new GuidWrapper(measureId.ToString());
        }

        [ContextMethod("StopPerformanceMeasure", "ЗавершитьЗамерПроизводительности")]
        public CoverageData StopPerformanceMeasure() => 
            StopPerformanceMeasureConfigureAwait().GetAwaiter().GetResult();

        private async Task<CoverageData> StopPerformanceMeasureConfigureAwait() =>
            await StopPerformanceMeasureAsync().ConfigureAwait(false);

        private async Task<CoverageData> StopPerformanceMeasureAsync()
        {
            await DebuggerSession.SetMeasureModeAsync(Guid.Empty);
            await DebuggerSession.PingAsync();
            return coverageData;
        }

        private async Task HandlerTargetStartedAsync(DebugTargetId targetID)
        {
            await DebuggerSession.AttachDebugTargetAsync(targetID.TargetIdLight);
        }

        private async Task HandlerTargetQuitAsync(DebugTargetId targetID)
        {
            await DebuggerSession.DetachDebugTargetAsync(targetID.TargetIdLight);
        }

        private Task HandlerMeasureProcessingAsync(PerformanceInfoMain performanceInfo)
        {
            lock (coverageData)
            {
                coverageData.TotalDurability += performanceInfo.TotalDurability;
                performanceInfo.ModuleData.ForEach(x => ProcessPerformanceInfoModule(x));
            }

            return Task.CompletedTask;
        }

        private void ProcessPerformanceInfoModule(PerformanceInfoModule module)
        {
            var moduleBSL = new CoverageModuleId(module.ModuleID);
            var linesCoverage = coverageData.Data.Retrieve(moduleBSL) as MapImpl;
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
