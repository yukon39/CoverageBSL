using com.github.yukon39.CoverageBSL;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    internal static class CoverableLinesPreparer
    {
        public static void PrepareModule(PerformanceInfoModule module, IEnumerable<int> lineNumbers)
        {
            var existingLines = new HashSet<int>(module.LineInfo.Select(x => x.LineNo));

            foreach (var lineNo in lineNumbers)
            {
                if (existingLines.Add(lineNo))
                {
                    module.LineInfo.Add(new PerformanceInfoLine
                    {
                        LineNo = lineNo,
                        Frequency = 0
                    });
                }
            }

            module.LineInfo.Sort((a, b) => a.LineNo.CompareTo(b.LineNo));
        }

        public static void PrepareData(ICoverageData data, string moduleUrl, IEnumerable<int> lineNumbers)
        {
            foreach (var performanceInfo in data.Data)
            {
                foreach (var module in performanceInfo.ModuleData)
                {
                    if (module.ModuleID.URL.Equals(moduleUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        PrepareModule(module, lineNumbers);
                        return;
                    }
                }
            }
        }
    }
}
