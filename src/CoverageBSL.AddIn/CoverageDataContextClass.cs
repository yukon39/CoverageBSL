using ScriptEngine.HostedScript.Library;
using ScriptEngine.Machine.Contexts;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [ContextClass(typeName: "CoverageData", typeAlias: "ДанныеПокрытия")]
    public class CoverageDataContextClass : AutoContext<CoverageDataContextClass>
    {
        [ContextProperty("TotalDurability")]
        public long TotalDurability { get; }

        [ContextProperty("Data")]
        public MapImpl Data { get; }

        public CoverageDataContextClass(ICoverageData coverageData)
        {
            TotalDurability = coverageData.TotalDurability;
            Data = coverageData.Data;
        }
    }
}
