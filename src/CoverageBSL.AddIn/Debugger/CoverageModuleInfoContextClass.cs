using com.github.yukon39.CoverageBSL.AddIn.Utils;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
using ScriptEngine.Machine.Contexts;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageModuleInfo", typeAlias: "ПокрытиеСтрокМодуля")]
    public class CoverageModuleInfoContextClass : AutoContext<CoverageModuleInfoContextClass>
    {
        public CoverageModuleInfoContextClass(PerformanceInfoModule moduleInfo)
        {
            ModuleId = new CoverageModuleIdWrapper(moduleInfo.ModuleID);
            LineInfo = new CoverageLineInfosContextClass(moduleInfo.LineInfo);
        }

        [ScriptConstructor]
        public static CoverageModuleInfoContextClass ScriptConstructor()
            => new CoverageModuleInfoContextClass();

        private CoverageModuleInfoContextClass()
            => LineInfo = new CoverageLineInfosContextClass();

        [ContextProperty("ModuleId", "ОписаниеМодуля")]
        public CoverageModuleIdWrapper ModuleId { get; set; }

        [ContextProperty("LineInfo", "ПокрытиеМодуля")]
        public CoverageLineInfosContextClass LineInfo { get; }

        [ContextMethod("SerializeJSON", "СериализоватьJSON")]
        public void SerializeJson(JSONWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(ModuleId));
            ModuleId.SerializeJson(writer);

            writer.WritePropertyName(nameof(LineInfo));
            LineInfo.SerializeJson(writer);

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
                    case nameof(ModuleId):
                        ModuleId = CoverageModuleIdWrapper.ScriptConstructor();
                        ModuleId.DeserializeJson(reader);
                        break;
                    case nameof(LineInfo):
                        ReadLineInfos(reader, LineInfo);
                        break;
                    default:
                        JsonReaderHelper.SkipValue(reader);
                        break;
                }
            }

            throw new JSONReaderException("Unexpected end of JSON while reading coverage module");
        }

        private static void ReadLineInfos(JSONReader reader, CoverageLineInfosContextClass lineInfos)
        {
            JsonReaderHelper.ReadExpected(reader, JSONValueTypeEnum.ArrayStart);

            while (reader.Read())
            {
                if (reader.CurrentValueType == JSONValueTypeEnum.ArrayEnd)
                {
                    return;
                }

                var lineInfo = CoverageLineInfoWrapper.ScriptConstructor();
                lineInfo.DeserializeJson(reader);
                lineInfos.Add(lineInfo);
            }
        }
    }
}
