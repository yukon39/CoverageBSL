using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;

namespace com.github.yukon39.DebugBSL.Client
{
    public interface IDebuggerClientMeasure
    {
        Task StartMeasureModeAsync(Guid measureMode);

        Task<Guid> StartMeasureModeAsync();

        Task<List<PerformanceInfoMain>> StopMeasureModeAsync();
    }
}
