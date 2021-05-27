using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL
{
    public interface ICoverageData
    {
        long TotalDurability { get; set; }
        List<PerformanceInfoMain> Data { get; }

        void AddRange(List<PerformanceInfoMain> performanceInfos);
    }
}
