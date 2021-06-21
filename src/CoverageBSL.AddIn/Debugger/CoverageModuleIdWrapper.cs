using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using ScriptEngine.HostedScript.Library.Json;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine.Values;
using System;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageModuleId", typeAlias: "МодульПокрытия")]
    public class CoverageModuleIdWrapper : AutoContext<CoverageModuleIdWrapper>, IObjectWrapper
    {
        private readonly BSLModuleIdInternal moduleId;

        public CoverageModuleIdWrapper(BSLModuleIdInternal moduleId)
            => this.moduleId = moduleId;

        [ScriptConstructor]
        public static CoverageModuleIdWrapper ScriptConstructor()
            => new CoverageModuleIdWrapper();

        private CoverageModuleIdWrapper()
            => moduleId = new BSLModuleIdInternal();

        [ContextProperty("ModuleType", "ТипМодуля")]
        public string ModuleType
        {
            get => moduleId.Type.ToString();
            set => moduleId.Type = (BSLModuleType)Enum.Parse(typeof(BSLModuleType), value);
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
            set => moduleId.ObjectID = Guid.Parse(value);
        }

        [ContextProperty("PropertyID", "ИдентификаторСвойства")]
        public string PropertyID
        {
            get => moduleId.PropertyID.ToString();
            set => moduleId.PropertyID = Guid.Parse(value);
        }

        [ContextMethod("SerializeJSON", "СериализоватьJSON")]
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

            writer.WriteEndObject();
        }

        public object UnderlyingObject
        {
            get => moduleId;
        }
    }
}
