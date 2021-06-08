using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using ScriptEngine.HostedScript.Library.Json;
using ScriptEngine.Machine.Contexts;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageModuleInfo", typeAlias: "ПокрытиеСтрокМодуля")]
    class CoverageModuleInfoContextClass : AutoContext<CoverageModuleInfoContextClass>
    {
        private readonly CoverageModuleIdWrapper moduleId;
        private readonly CoverageLineInfosContextClass lineInfos;

        public CoverageModuleInfoContextClass(PerformanceInfoModule moduleInfo)
        {
            moduleId = new CoverageModuleIdWrapper(moduleInfo.ModuleID);
            lineInfos = new CoverageLineInfosContextClass(moduleInfo.LineInfo);
        }

        public void SerializeJson(JSONWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("ModuleId");
            moduleId.SerializeJson(writer);

            writer.WritePropertyName("LineInfo");
            lineInfos.SerializeJson(writer);

            writer.WriteEndObject();
        }
    }
}
