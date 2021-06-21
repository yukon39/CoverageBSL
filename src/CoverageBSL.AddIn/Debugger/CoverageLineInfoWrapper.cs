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

        [ContextProperty("Frequency", "Частотность")]
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

        [ContextProperty("PureDurability", "ЧистаяПродолжительность")]
        public long PureDurability
        {
            get => lineInfo.PureDurability;
            set => lineInfo.PureDurability = value;
        }

        [ContextMethod("SerializeJSON", "СериализоватьJSON")]
        public void SerializeJson(JSONWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(LineNo));
            writer.WriteValue(NumberValue.Create(LineNo));

            writer.WritePropertyName(nameof(Frequency));
            writer.WriteValue(NumberValue.Create(Frequency));

            writer.WritePropertyName(nameof(Durability));
            writer.WriteValue(NumberValue.Create((decimal)Durability));

            writer.WritePropertyName(nameof(PureDurability));
            writer.WriteValue(NumberValue.Create((decimal)PureDurability));

            writer.WriteEndObject();
        }

        public object UnderlyingObject
        {
            get => lineInfo;
        }
    }
}
