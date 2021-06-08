using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using ScriptEngine.HostedScript.Library.Json;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine.Values;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageModuleId", typeAlias: "МодульПокрытия")]
    class CoverageModuleIdWrapper : AutoContext<CoverageLineInfoWrapper>, IObjectWrapper
    {
        private readonly BSLModuleIdInternal moduleId;

        public CoverageModuleIdWrapper(BSLModuleIdInternal moduleId)
            => this.moduleId = moduleId;

        [ContextProperty("ModuleType", "ТипМодуля")]
        public string ModuleType
        {
            get => moduleId.Type.ToString();
        }

        [ContextProperty("URL")]
        public string URL
        {
            get => moduleId.URL;
            set => moduleId.URL = value;
        }

        [ContextProperty("ExtensionName", "ИмяРасширения")]
        public string ExtensionName
        {
            get => moduleId.ExtensionName;
            set => moduleId.ExtensionName = value;
        }

        [ContextProperty("ObjectID", "ИдентификаторОбъекта")]

        public string ObjectID
        {
            get => moduleId.ObjectID.ToString();
        }

        [ContextProperty("PropertyID", "ИдентификаторСвойства")]
        public string PropertyID
        {
            get => moduleId.PropertyID.ToString();
        }

        [ContextProperty("ExtId", "ИдентификаторРасширения")]
        public int ExtId
        {
            get => moduleId.ExtId;
            set => moduleId.ExtId = value;
        }

        [ContextProperty("Version", "Версия")]
        public string Version
        {
            get => moduleId.Version;
            set => moduleId.Version = value;
        }

        public void SerializeJson(JSONWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(ModuleType));
            writer.WriteValue(StringValue.Create(ModuleType));

            writer.WritePropertyName(nameof(URL));
            writer.WriteValue(StringValue.Create(URL));

            writer.WritePropertyName(nameof(ExtensionName));
            writer.WriteValue(StringValue.Create(ExtensionName));

            writer.WritePropertyName(nameof(ObjectID));
            writer.WriteValue(StringValue.Create(ObjectID));

            writer.WritePropertyName(nameof(PropertyID));
            writer.WriteValue(StringValue.Create(PropertyID));

            writer.WritePropertyName(nameof(ExtId));
            writer.WriteValue(NumberValue.Create(ExtId));

            writer.WritePropertyName(nameof(Version));
            writer.WriteValue(StringValue.Create(Version));

            writer.WriteEndObject();
        }

        public object UnderlyingObject
        {
            get => moduleId;
        }
    }
}
