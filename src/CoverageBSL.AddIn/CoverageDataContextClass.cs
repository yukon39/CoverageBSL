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
        {
            performanceInfos.ForEach(x => ProcessPerformanceInfo(x));
        }

        private void ProcessPerformanceInfo(PerformanceInfoMain info)
        {
            info.ModuleData.ForEach(x => ProcessPerformanceInfoModule(x));
        }

        private void ProcessPerformanceInfoModule(PerformanceInfoModule module)
        {
            var moduleBSL = ModuleId(module.ModuleID);
            var linesCoverage = new ArrayImpl();

            module.LineInfo.ForEach(x => ProcessPerformanceInfoLine(x, linesCoverage));

            moduleBSL.Insert("LineNo", linesCoverage);
            Data.Add(moduleBSL);
        }

        private static void ProcessPerformanceInfoLine(PerformanceInfoLine line, ArrayImpl linesCoverage)
        {
            var result = new StructureImpl();
            result.Insert("lineNumber", NumberValue.Create(line.LineNo));
            result.Insert("covered", BooleanValue.True);

            linesCoverage.Add(result);
        }

        private static StructureImpl ModuleId(BSLModuleIdInternal moduleId)
        {
            var result = new StructureImpl();
            result.Insert("ModuleType", StringValue.Create(moduleId.Type.ToString()));
            result.Insert("URL", StringValue.Create(moduleId.URL));
            result.Insert("ExtensionName", StringValue.Create(moduleId.ExtensionName));
            result.Insert("ObjectID", StringValue.Create(moduleId.ObjectID.ToString()));
            result.Insert("PropertyID", StringValue.Create(moduleId.PropertyID.ToString()));
            result.Insert("ExtId", NumberValue.Create(moduleId.ExtId));
            result.Insert("Version", StringValue.Create(moduleId.Version));

            return result;
        }
    }
}
