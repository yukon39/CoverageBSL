using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.github.yukon39.DebugBSL.Client.Data;
using com.github.yukon39.DebugBSL.Client.Internal;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;

namespace com.github.yukon39.DebugBSL.Client.Impl
{
    public class DebuggerClientMeasureManager : DebuggerClientEntityManager, IDebuggerClientMeasure
    {
        private IDebuggerClientSession Session;
        private readonly List<PerformanceInfoMain> PerformanceInfo = new List<PerformanceInfoMain>();
        private Guid MeasureId = Guid.Empty;

        public DebuggerClientMeasureManager(DebuggerClientExecutor executor, SessionContext context)
            : base(executor, context) { }

        public async Task StartMeasureModeAsync(Guid measureId)
        {
            if (MeasureId.Equals(Guid.Empty))
            {
                MeasureId = measureId;
            }
            else
            {
                throw new InvalidOperationException();
            }

            PerformanceInfo.Clear();
            await SetMeasureModeAsync(MeasureId);
        }

        public async Task<Guid> StartMeasureModeAsync()
        {
            if (MeasureId.Equals(Guid.Empty))
            {
                MeasureId = Guid.NewGuid();
            }
            else
            {
                throw new InvalidOperationException();
            }

            PerformanceInfo.Clear();
            await SetMeasureModeAsync(MeasureId);

            return MeasureId;
        }

        public async Task<List<PerformanceInfoMain>> StopMeasureModeAsync()
        {
            if (MeasureId.Equals(Guid.Empty))
            {
                throw new InvalidOperationException();
            }

            MeasureId = Guid.Empty;
            await SetMeasureModeAsync(MeasureId);
            await Session.PingAsync();

            var result = new List<PerformanceInfoMain>();
            result.AddRange(PerformanceInfo);

            PerformanceInfo.Clear();

            return result;
        }

        private async Task SetMeasureModeAsync(Guid measureModeSeanceID)
        {
            var requestParameters = new RequestParameters("setMeasureMode");

            var request = Context.NewSessionRequest<RDBGSetMeasureModeRequest>();
            request.MeasureModeSeanceID = measureModeSeanceID;

            await Executor.ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);
        }

        public override void SubscribeSessionEvents(IDebuggerClientSession session)
        {
            Session = session;
            Session.MeasureProcessing += MeasureProcessingHandler;
        }

        public Task MeasureProcessingHandler(IDebuggerClientSession sender, PerformanceInfoMain performanceInfo)
        {
            PerformanceInfo.Add(performanceInfo);
            return Task.CompletedTask;
        }
    }
}
