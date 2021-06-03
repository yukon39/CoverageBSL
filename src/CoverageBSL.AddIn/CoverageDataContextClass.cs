using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using ScriptEngine.HostedScript.Library;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine.Values;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [ContextClass(typeName: "CoverageData", typeAlias: "ДанныеПокрытия")]
    public class CoverageDataContextClass : AutoContext<CoverageDataContextClass>
    {
        [ContextProperty("TotalDurability")]
        public long TotalDurability { get; }

        [ContextProperty("Data")]
        public ArrayImpl Data { get; } = new ArrayImpl();

        public CoverageDataContextClass(ICoverageData coverageData)
        {
            TotalDurability = coverageData.TotalDurability;
            AddRange(coverageData.Data);
        }

        private void AddRange(List<PerformanceInfoMain> performanceInfos) 
            => performanceInfos.ForEach(x => ProcessPerformanceInfo(x));

        private void ProcessPerformanceInfo(PerformanceInfoMain info) 
            => info.ModuleData.ForEach(x => ProcessPerformanceInfoModule(x));

        private void ProcessPerformanceInfoModule(PerformanceInfoModule module) 
            => Data.Add(new CoverageModuleInfoContextClass(module));
    }
}
