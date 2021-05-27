using ScriptEngine.HostedScript.Library;
using ScriptEngine.Machine.Contexts;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [ContextClass(typeName: "CoverageData", typeAlias: "ДанныеПокрытия")]
    public class CoverageDataContextClass : AutoContext<CoverageDataContextClass>, IObjectWrapper
    {
        private readonly ICoverageData coverageData;

        public CoverageDataContextClass(ICoverageData coverageData) =>
            this.coverageData = coverageData;

        [ContextProperty("TotalDurability")]
        public long TotalDurability
        {
            get => coverageData.TotalDurability;
            set => coverageData.TotalDurability = value;
        }

        [ContextProperty("Data")]
        public MapImpl Data { get; } = new MapImpl();

        [ContextMethod("Write")]
        public void Write(string filePath) =>
            coverageData.Write(filePath);

        public object UnderlyingObject => coverageData;
    }
}
