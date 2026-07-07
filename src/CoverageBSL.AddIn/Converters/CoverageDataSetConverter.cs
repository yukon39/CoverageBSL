using System.Linq;
using com.github.yukon39.CoverageBSL.AddIn.CoverageData;
using com.github.yukon39.CoverageBSL.AddIn.Debugger;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal static class CoverageDataSetConverter
    {
        public static CoverageDataSet Convert(CoverageDataContextClass coverageData)
        {
            var result = new CoverageDataSet();

            foreach (var moduleInfo in coverageData.Data.Cast<CoverageModuleInfoContextClass>())
            {
                var module = result.GetOrAdd(CoverageDataSetModuleKeyBuilder.Build(moduleInfo.ModuleId));

                foreach (var lineInfo in moduleInfo.LineInfo.Cast<CoverageLineInfoWrapper>())
                {
                    module.MarkCovered(lineInfo.LineNo);
                }
            }

            return result;
        }
    }
}
