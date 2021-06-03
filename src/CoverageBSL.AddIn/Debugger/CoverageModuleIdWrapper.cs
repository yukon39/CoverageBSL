using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using ScriptEngine.Machine.Contexts;

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

        public object UnderlyingObject
        {
            get => moduleId;
        }
    }
}
