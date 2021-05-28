using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.Impl
{
    public class CoverageData : ICoverageData
    {
        public long TotalDurability { get; set; }

        public int TotalLines { get; set; }

        public List<PerformanceInfoMain> Data { get; } = new List<PerformanceInfoMain>();

        public void AddRange(List<PerformanceInfoMain> performanceInfos) =>
            performanceInfos.ForEach(x => ProcessPerformanceInfo(x));

        private void ProcessPerformanceInfo(PerformanceInfoMain info)
        {
            TotalDurability += info.TotalDurability;
            Data.Add(info);
        }
    }
}