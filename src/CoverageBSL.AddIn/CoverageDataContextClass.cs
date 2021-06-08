using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using ScriptEngine.HostedScript.Library.Json;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine.Values;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [ContextClass(typeName: "CoverageData", typeAlias: "ДанныеПокрытия")]
    public class CoverageDataContextClass : AutoContext<CoverageDataContextClass>
    {
        [ContextProperty("TotalDurability")]
        public long TotalDurability { get; }

        [ContextProperty("Data")]
        public CoverageModuleInfoList Data { get; }
        public CoverageDataContextClass(ICoverageData coverageData)
        {
            TotalDurability = coverageData.TotalDurability;
            Data = new CoverageModuleInfoList(coverageData.Data);
        }
    }
}
