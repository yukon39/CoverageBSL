using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using ScriptEngine.Machine.Contexts;

#if NET48
using ScriptEngine.HostedScript.Library.Json;
#else
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
#endif

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageModuleInfo", typeAlias: "ПокрытиеСтрокМодуля")]
    public class CoverageModuleInfoContextClass : AutoContext<CoverageModuleInfoContextClass>
    {
        public CoverageModuleInfoContextClass(PerformanceInfoModule moduleInfo)
        {
            ModuleId = new CoverageModuleIdWrapper(moduleInfo.ModuleID);
            LineInfo = new CoverageLineInfosContextClass(moduleInfo.LineInfo);
        }

        [ScriptConstructor]
        public static CoverageModuleInfoContextClass ScriptConstructor()
            => new CoverageModuleInfoContextClass();

        private CoverageModuleInfoContextClass()
            => LineInfo = new CoverageLineInfosContextClass();

        [ContextProperty("ModuleId", "ОписаниеМодуля")]
        public CoverageModuleIdWrapper ModuleId { get; set; }

        [ContextProperty("LineInfo", "ПокрытиеМодуля")]
        public CoverageLineInfosContextClass LineInfo { get; }

        [ContextMethod("SerializeJSON", "СериализоватьJSON")]
        public void SerializeJson(JSONWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(ModuleId));
            ModuleId.SerializeJson(writer);

            writer.WritePropertyName(nameof(LineInfo));
            LineInfo.SerializeJson(writer);

            writer.WriteEndObject();
        }
    }
}
