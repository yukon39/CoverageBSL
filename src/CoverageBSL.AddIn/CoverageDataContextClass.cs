using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Utils;
using ScriptEngine.Machine.Contexts;

#if NET48
using ScriptEngine.HostedScript.Library.Json;
#else
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
#endif

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
            JSONUtils.WriteValue(writer, TotalDurability);

            writer.WritePropertyName(nameof(Data));
            Data.SerializeJson(writer);

            writer.WriteEndObject();
        }
    }
}
