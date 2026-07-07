using System.Collections.Generic;
using System.Linq;
using com.github.yukon39.CoverageBSL.AddIn.CoverageData;
using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Internal;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal static class CoverageDataSetConverter
    {
        public static CoverageDataSet Convert(
            CoverageDataContextClass coverageData,
            string workingArea,
            IEnumerable<SourceDirectoryData> sourceDirectories)
        {
            var result = new CoverageDataSet();

            foreach (var moduleInfo in coverageData.Data.Cast<CoverageModuleInfoContextClass>())
            {
                var moduleUrl = CoverageDataSetModuleKeyBuilder.Build(
                    moduleInfo.ModuleId,
                    workingArea,
                    sourceDirectories);
                if (moduleUrl == null)
                {
                    continue;
                }

                var module = result.GetOrAdd(moduleUrl);

                foreach (var lineInfo in moduleInfo.LineInfo.Cast<CoverageLineInfoWrapper>())
                {
                    module.MarkCovered(lineInfo.LineNo);
                }
            }

            return result;
        }
    }
}
