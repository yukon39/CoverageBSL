using com.github.yukon39.CoverageBSL;
using com.github.yukon39.CoverageBSL.AddIn.CoverageData;
using com.github.yukon39.CoverageBSL.Impl;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    internal static class CoverageDataConverter
    {
        public static ICoverageData ToCoverageData(CoverageDataSet source)
        {
            var performanceInfo = new PerformanceInfoMain();

            foreach (var module in source.Modules)
            {
                performanceInfo.ModuleData.Add(ToPerformanceInfoModule(module));
            }

            var coverageData = new CoverageBSL.Impl.CoverageData();
            coverageData.Data.Add(performanceInfo);
            return coverageData;
        }

        private static PerformanceInfoModule ToPerformanceInfoModule(CoverageModuleData source)
        {
            var module = new PerformanceInfoModule
            {
                ModuleID = new BSLModuleIdInternal
                {
                    Type = BSLModuleType.ExtMDModule,
                    URL = source.ModuleUrl
                }
            };

            foreach (var line in source.LineInfo)
            {
                module.LineInfo.Add(new PerformanceInfoLine
                {
                    LineNo = line.LineNo,
                    Frequency = line.HasCoverage ? 1 : 0
                });
            }

            return module;
        }
    }
}
