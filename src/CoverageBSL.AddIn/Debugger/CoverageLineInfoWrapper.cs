using com.github.yukon39.CoverageBSL.AddIn.Utils;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using OneScript.Commons;
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
using ScriptEngine.Machine.Contexts;

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

        [ContextMethod("DeserializeJSON", "ДесериализоватьJSON")]
        public void DeserializeJson(JSONReader reader)
        {
            JsonReaderHelper.EnsureValueType(reader, JSONValueTypeEnum.ObjectStart);

            while (reader.Read())
            {
                if (reader.CurrentValueType == JSONValueTypeEnum.ObjectEnd)
                {
                    return;
                }

                var propertyName = JsonReaderHelper.ReadPropertyName(reader);
                switch (propertyName)
                {
                    case nameof(LineNo):
                        LineNo = JsonReaderHelper.ReadInt32(reader);
                        break;
                    case nameof(Frequency):
                        Frequency = JsonReaderHelper.ReadInt32(reader);
                        break;
                    case nameof(Durability):
                        Durability = JsonReaderHelper.ReadInt64(reader);
                        break;
                    case nameof(PureDurability):
                        PureDurability = JsonReaderHelper.ReadInt64(reader);
                        break;
                    default:
                        JsonReaderHelper.SkipValue(reader);
                        break;
                }
            }

            throw new JSONReaderException("Unexpected end of JSON while reading line info");
        }

        public object UnderlyingObject
        {
            get => lineInfo;
        }
    }
}
