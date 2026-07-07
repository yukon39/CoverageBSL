using com.github.yukon39.CoverageBSL.AddIn.Converters;
using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Utils;
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
using ScriptEngine.Machine.Contexts;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [ContextClass(typeName: "CoverageData", typeAlias: "ДанныеПокрытия")]
    public class CoverageDataContextClass : AutoContext<CoverageDataContextClass>
    {
        private readonly ICoverageData coverageData;

        public CoverageDataContextClass(ICoverageData coverageData)
        {
            this.coverageData = coverageData;
            TotalDurability = coverageData.TotalDurability;
            Data = new CoverageModuleInfoList(coverageData.Data);
        }

        [ScriptConstructor]
        public static CoverageDataContextClass ScriptConstructor()
            => new CoverageDataContextClass();

        private CoverageDataContextClass()
            => Data = new CoverageModuleInfoList();

        [ContextProperty("TotalDurability", "ОбщаяПродолжительность")]
        public long TotalDurability { get; set; }

        [ContextProperty("Data", "Данные")]
        public CoverageModuleInfoList Data { get; private set; }

        [ContextMethod("DeserializeJSON", "ДесериализоватьJSON")]
        public void DeserializeJson(JSONReader reader)
        {
            JsonReaderHelper.ReadExpected(reader, JSONValueTypeEnum.ObjectStart);
            Data = new CoverageModuleInfoList();

            while (reader.Read())
            {
                if (reader.CurrentValueType == JSONValueTypeEnum.ObjectEnd)
                {
                    return;
                }

                var propertyName = JsonReaderHelper.ReadPropertyName(reader);
                switch (propertyName)
                {
                    case nameof(TotalDurability):
                        TotalDurability = JsonReaderHelper.ReadInt64(reader);
                        break;
                    case nameof(Data):
                        Data.DeserializeJson(reader);
                        break;
                    default:
                        JsonReaderHelper.SkipValue(reader);
                        break;
                }
            }
        }

        [ContextMethod("SerializeJSON", "СериализоватьJSON")]
        public void SerializeJson(JSONWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(TotalDurability));
            JSONUtils.WriteValue(writer, TotalDurability);

            writer.WritePropertyName(nameof(Data));
            Data.SerializeJson(writer);

            writer.WriteEndObject();
        }

        internal CoverageData.CoverageDataSet ToCoverageDataSet()
            => CoverageDataSetConverter.Convert(this);
    }
}
