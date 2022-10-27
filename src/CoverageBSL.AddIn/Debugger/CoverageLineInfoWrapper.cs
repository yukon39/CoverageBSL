using com.github.yukon39.CoverageBSL.AddIn.Utils;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using ScriptEngine.Machine.Contexts;

#if NET5_0_OR_GREATER
using OneScript.Commons;
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
#else
using ScriptEngine.HostedScript.Library.Json;
#endif

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
            JSONUtils.WriteValue(writer, LineNo);

            writer.WritePropertyName(nameof(Frequency));
            JSONUtils.WriteValue(writer, Frequency);

            writer.WritePropertyName(nameof(Durability));
            JSONUtils.WriteValue(writer, Durability);

            writer.WritePropertyName(nameof(PureDurability));
            JSONUtils.WriteValue(writer, PureDurability);

            writer.WriteEndObject();
        }

        public object UnderlyingObject
        {
            get => lineInfo;
        }
    }
}
