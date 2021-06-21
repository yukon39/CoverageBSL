using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using ScriptEngine.HostedScript.Library.Json;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine.Values;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [ContextClass(typeName: "CoverageData", typeAlias: "ДанныеПокрытия")]
    public class CoverageDataContextClass : AutoContext<CoverageDataContextClass>
    {
        public CoverageDataContextClass(ICoverageData coverageData)
        {
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

        [ContextMethod("SerializeJSON", "СериализоватьJSON")]
        public void SerializeJson(JSONWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(TotalDurability));
            writer.WriteValue(NumberValue.Create((decimal)TotalDurability));

            writer.WritePropertyName(nameof(Data));
            Data.SerializeJson(writer);

            writer.WriteEndObject();
        }
    }
}
