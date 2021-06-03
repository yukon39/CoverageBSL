using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using ScriptEngine.Machine.Contexts;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageLineInfo", typeAlias: "ПокрытиеСтрок")]
    class CoverageLineInfoWrapper : AutoContext<CoverageLineInfoWrapper>, IObjectWrapper
    {
        private readonly PerformanceInfoLine lineInfo;

        public CoverageLineInfoWrapper(PerformanceInfoLine lineInfo) =>
            this.lineInfo = lineInfo;

        [ContextProperty("LineNo", "НомерСтроки")]
        public int LineNo
        {
            get => lineInfo.LineNo;
            set => lineInfo.LineNo = value;
        }

        [ContextProperty("Hits", "Попаданий")]
        public int Frequency
        {
            get => lineInfo.Frequency;
            set => lineInfo.Frequency = value;
        }

        [ContextProperty("Durability", "Продолжительность")]
        public long Durability
        {
            get => lineInfo.Durability;
            set => lineInfo.Durability = value;
        }

        public object UnderlyingObject
        {
            get => lineInfo;
        }
    }
}
