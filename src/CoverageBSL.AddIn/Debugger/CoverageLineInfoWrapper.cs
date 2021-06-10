using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using ScriptEngine.HostedScript.Library.Json;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine.Values;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageLineInfo", typeAlias: "ПокрытиеСтрок")]
    public class CoverageLineInfoWrapper : AutoContext<CoverageLineInfoWrapper>, IObjectWrapper
    {
        private readonly PerformanceInfoLine lineInfo;

        public CoverageLineInfoWrapper(PerformanceInfoLine lineInfo) =>
            this.lineInfo = lineInfo;

        [ScriptConstructor]
        public static CoverageLineInfoWrapper ScriptConstructor()
            => new CoverageLineInfoWrapper();

        private CoverageLineInfoWrapper()
            => lineInfo = new PerformanceInfoLine();

        [ContextProperty("LineNo", "НомерСтроки")]
        public int LineNo
        {
            get => lineInfo.LineNo;
            set => lineInfo.LineNo = value;
        }

        [ContextProperty("Hits", "Попаданий")]
        public int Hits
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

        [ContextMethod("SerializeJSON", "СериализоватьJSON")]
        public void SerializeJson(JSONWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(LineNo));
            writer.WriteValue(NumberValue.Create(LineNo));

            writer.WritePropertyName(nameof(Hits));
            writer.WriteValue(NumberValue.Create(Hits));

            writer.WritePropertyName(nameof(Durability));
            writer.WriteValue(NumberValue.Create((decimal)Durability));

            writer.WriteEndObject();
        }

        public object UnderlyingObject
        {
            get => lineInfo;
        }
    }
}
