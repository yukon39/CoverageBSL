using com.github.yukon39.CoverageBSL.Coverage;
using ScriptEngine.HostedScript.Library;
using ScriptEngine.Machine.Contexts;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [ContextClass(typeName: "CoverageData", typeAlias: "ДанныеПокрытия")]
    public class CoverageDataWrapper : AutoContext<CoverageDataWrapper>, IObjectWrapper
    {
        private readonly CoverageData coverageData;

        public CoverageDataWrapper(CoverageData coverageData) =>
            this.coverageData = coverageData;

        [ContextProperty("TotalDurability")]
        public long TotalDurability
        {
            get => coverageData.TotalDurability;
            set => coverageData.TotalDurability = value;
        }

        [ContextProperty("Data")]
        public MapImpl Data
        {
            get => coverageData.Data;
        }

        [ContextMethod("Write")]
        public void Write(string filePath) =>
            coverageData.Write(filePath);

        public object UnderlyingObject => coverageData;
    }
}
