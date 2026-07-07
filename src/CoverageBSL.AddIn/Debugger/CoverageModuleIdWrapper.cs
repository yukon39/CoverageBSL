using com.github.yukon39.CoverageBSL.AddIn.Utils;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using OneScript.Commons;
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
using ScriptEngine.Machine.Contexts;
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
            JSONUtils.WriteValue(writer, ModuleType);

            writer.WritePropertyName(nameof(URL));
            JSONUtils.WriteValue(writer, URL);

            writer.WritePropertyName(nameof(ExtensionName));
            JSONUtils.WriteValue(writer, ExtensionName);

            writer.WritePropertyName(nameof(ObjectID));
            JSONUtils.WriteValue(writer, ObjectID);

            writer.WritePropertyName(nameof(PropertyID));
            JSONUtils.WriteValue(writer, PropertyID);

            writer.WriteEndObject();
        }

        [ContextMethod("DeserializeJSON", "ДесериализоватьJSON")]
        public void DeserializeJson(JSONReader reader)
        {
            JsonReaderHelper.ReadExpected(reader, JSONValueTypeEnum.ObjectStart);

            while (reader.Read())
            {
                if (reader.CurrentValueType == JSONValueTypeEnum.ObjectEnd)
                {
                    return;
                }

                var propertyName = JsonReaderHelper.ReadPropertyName(reader);
                switch (propertyName)
                {
                    case nameof(ModuleType):
                        ModuleType = JsonReaderHelper.ReadString(reader);
                        break;
                    case nameof(URL):
                        URL = JsonReaderHelper.ReadString(reader);
                        break;
                    case nameof(ExtensionName):
                        ExtensionName = JsonReaderHelper.ReadString(reader);
                        break;
                    case nameof(ObjectID):
                        ObjectID = JsonReaderHelper.ReadString(reader);
                        break;
                    case nameof(PropertyID):
                        PropertyID = JsonReaderHelper.ReadString(reader);
                        break;
                    default:
                        JsonReaderHelper.SkipValue(reader);
                        break;
                }
            }

            throw new JSONReaderException("Unexpected end of JSON while reading module id");
        }

        public object UnderlyingObject
        {
            get => moduleId;
        }
    }
}
