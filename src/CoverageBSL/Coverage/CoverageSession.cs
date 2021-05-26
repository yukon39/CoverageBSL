using com.github.yukon39.CoverageBSL.Utils;
using com.github.yukon39.DebugBSL.Client;
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
using System.Threading;
using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL.Coverage
{
    [ContextClass(typeName: "CoverageSession", typeAlias: "СессияПокрытия")]
    public class CoverageSession : AutoContext<CoverageSession>, IDisposable
    {
        private readonly IDebuggerClientSession DebuggerSession;
        private readonly List<DebugTargetType> TargetTypes = DefaultTargetTypes;
        private readonly List<string> AreaNames = new List<string>();
        private readonly SemaphoreSlim CoverageSemaphore = new SemaphoreSlim(1, 1);
        private CoverageData coverageData;
        public CoverageSession(IDebuggerClient debuggerClient, string infobaseAlias)
        {
            DebuggerSession = debuggerClient.CreateSession(infobaseAlias);

            DebuggerSession.MeasureProcessing += HandlerMeasureProcessingAsync;
        }

        [ContextMethod("Attach", "Подключить")]
        public void Attach(string password)
        {
            try
            {
                AttachConfigureAwait(password).GetAwaiter().GetResult();
            }
            catch (RuntimeException rex)
            {
                Logger.Error(rex.ErrorDescription, rex);
                throw;
            }
            catch (Exception ex)
            {
                var message = Locale.NStr("en = 'Attach error';ru = 'Ошибка подключения'");
                Logger.Error(message, ex);
                throw RuntimeExceptionFactory.NewException(message, ex);
            }
        }

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

            var targetsManager = DebuggerSession.GetTargetsManager();

            await targetsManager.InitSettingsAsync(data);
            await targetsManager.SetAutoAttachSettingsAsync(autoAttachSettings);
            (await targetsManager.AttachedTargetsStatesAsync(""))
                .ForEach(async x => await targetsManager.AttachDebugTargetAsync(x.TargetID.TargetIdLight));
        }

        [ContextMethod("Detach", "Отключить")]
        public void Detach()
        {
            try
            {
                DetachConfigureAwait().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var message = Locale.NStr("en = 'Detach error';ru = 'Ошибка отключения'");
                Logger.Error(message, ex);
                throw RuntimeExceptionFactory.NewException(message, ex);
            }
        }

        private async Task DetachConfigureAwait() =>
            await DebuggerSession.DetachAsync().ConfigureAwait(false);

        [ContextMethod("StartPerformanceMeasure", "НачатьЗамерПроизводительности")]
        public GuidWrapper StartPerformanceMeasure()
        {
            try
            {
                return StartPerformanceMeasureConfigureAwait().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var message = Locale.NStr("en = 'StartPerformanceMeasure error';ru = 'Ошибка начала замера производительности'");
                Logger.Error(message, ex);
                throw RuntimeExceptionFactory.NewException(message, ex);
            }
        }

        private async Task<GuidWrapper> StartPerformanceMeasureConfigureAwait() =>
            await StartPerformanceMeasureAsync().ConfigureAwait(false);

        private async Task<GuidWrapper> StartPerformanceMeasureAsync()
        {
            var measureManager = DebuggerSession.GetMeasureManager();
            var measureId = await measureManager.StartMeasureModeAsync();

            await CoverageSemaphore.WaitAsync();
            coverageData = new CoverageData();
            CoverageSemaphore.Release();

            return new GuidWrapper(measureId.ToString());
        }

        [ContextMethod("StopPerformanceMeasure", "ЗавершитьЗамерПроизводительности")]
        public CoverageData StopPerformanceMeasure()
        {
            try
            {
                return StopPerformanceMeasureConfigureAwait().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var message = Locale.NStr("en = 'StopPerformanceMeasure error';ru = 'Ошибка завершения замера производительности'");
                Logger.Error(message, ex);
                throw RuntimeExceptionFactory.NewException(message, ex);
            }
        }

        private async Task<CoverageData> StopPerformanceMeasureConfigureAwait() =>
            await StopPerformanceMeasureAsync().ConfigureAwait(false);

        private async Task<CoverageData> StopPerformanceMeasureAsync()
        {
            var measureManager = DebuggerSession.GetMeasureManager();
            await measureManager.StopMeasureModeAsync();
            await DebuggerSession.PingAsync();

            await CoverageSemaphore.WaitAsync();
            var result = coverageData;
            coverageData = new CoverageData();
            CoverageSemaphore.Release();

            return result;
        }
                
        private async Task HandlerMeasureProcessingAsync(IDebuggerClientSession sender, PerformanceInfoMain performanceInfo)
        {
            try
            {
                await CoverageSemaphore.WaitAsync();
                coverageData.TotalDurability += performanceInfo.TotalDurability;
                performanceInfo.ModuleData.ForEach(x => ProcessPerformanceInfoModule(x));
                CoverageSemaphore.Release();
            }
            catch (Exception ex)
            {
                var message = Locale.NStr(
                    "en = 'MeasureProcessing event handler error';" +
                    "ru = 'Ошибка обработки события MeasureProcessing'");
                Logger.Error(message, ex);
            }
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

        private void ProcessPerformanceInfoLine(PerformanceInfoLine line, MapImpl lineslinesCoverage) =>
            lineslinesCoverage.Insert(NumberValue.Create(line.LineNo), BooleanValue.True);

        private static List<DebugTargetType> DefaultTargetTypes => new List<DebugTargetType>()
        {
            DebugTargetType.Client,
            DebugTargetType.ManagedClient,
            DebugTargetType.WEBClient,
            DebugTargetType.Server,
            DebugTargetType.ServerEmulation
        };

        public void Dispose()
        {
            CoverageSemaphore.Dispose();
        }
    }
}
