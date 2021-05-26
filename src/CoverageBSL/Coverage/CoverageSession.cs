using com.github.yukon39.DebugBSL.Client;
using com.github.yukon39.DebugBSL.debugger.debugAutoAttach;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL.Coverage
{

    public class CoverageSession
    {
        private readonly IDebuggerClientSession DebuggerSession;
        private readonly List<DebugTargetType> TargetTypes = DefaultTargetTypes;
        private readonly List<string> AreaNames = new List<string>();

        public CoverageSession(IDebuggerClient debuggerClient, string infobaseAlias) =>
            DebuggerSession = debuggerClient.CreateSession(infobaseAlias);

        public AttachDebugUIResult Attach(string password)
        {
            var safePassword = password.ToCharArray();
            var options = new DebuggerOptions();
            return AttachConfigureAwait(safePassword, options).GetAwaiter().GetResult();
        }

        private async Task<AttachDebugUIResult> AttachConfigureAwait(char[] password, DebuggerOptions options) =>
            await AttachAsync(password, options).ConfigureAwait(false);

        private async Task<AttachDebugUIResult> AttachAsync(char[] password, DebuggerOptions options)
        {
            var result = await DebuggerSession.AttachAsync(password, options).ConfigureAwait(false);

            if (DebuggerSession.IsAttached())
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

            return result;
        }

        public void Detach() =>
             DetachConfigureAwait().GetAwaiter().GetResult();

        private async Task DetachConfigureAwait() =>
            await DebuggerSession.DetachAsync().ConfigureAwait(false);

        public Guid StartCoverageCapture() =>
            StartPerformanceMeasureConfigureAwait().GetAwaiter().GetResult();

        private async Task<Guid> StartPerformanceMeasureConfigureAwait() =>
            await DebuggerSession.GetMeasureManager().StartMeasureModeAsync().ConfigureAwait(false);

        public CoverageData StopCoverageCapture()
            => StopCoverageCaptureConfigureAwait().GetAwaiter().GetResult();

        private async Task<CoverageData> StopCoverageCaptureConfigureAwait() =>
            await StopCoverageCaptureAsync().ConfigureAwait(false);

        private async Task<CoverageData> StopCoverageCaptureAsync()
        {
            var measureManager = DebuggerSession.GetMeasureManager();
            var performanceInfo = await measureManager.StopMeasureModeAsync();

            var coverageData = new CoverageData();
            coverageData.AddRange(performanceInfo);
            return coverageData;
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
