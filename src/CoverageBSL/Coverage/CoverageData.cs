using ScriptEngine.HostedScript.Library;
using ScriptEngine.Machine.Contexts;

namespace com.github.yukon39.CoverageBSL.Coverage
{
    [ContextClass(typeName: "CoverageData", typeAlias: "ДанныеОтладки")]
    public class CoverageData : AutoContext<CoverageData>
    {
        [ContextProperty("TotalDurability")]
        public long TotalDurability { get; set; }

        public int TotalLines { get; set; }

        [ContextProperty("Data")]
        public MapImpl Data { get; set; }
    }
}
