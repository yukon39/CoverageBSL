using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using ScriptEngine.HostedScript.Library;
using ScriptEngine.Machine.Contexts;

namespace com.github.yukon39.CoverageBSL.Coverage
{
    [ContextClass(typeName: "CoverageModuleId", typeAlias: "ИдентификаторМодуляОтладки")]
    public class CoverageModuleId : AutoContext<CoverageModuleId>
    {
        public CoverageModuleId(BSLModuleIdInternal bslModuleId)
        {
            ModuleType = bslModuleId.Type.ToString();
            URL = bslModuleId.URL;
            ExtensionName = bslModuleId.ExtensionName;
            ExtId = bslModuleId.ExtId;
            ObjectID = new GuidWrapper(bslModuleId.ObjectID.ToString());
            PropertyID = new GuidWrapper(bslModuleId.ObjectID.ToString());
            Version = bslModuleId.Version;
        }

        [ContextProperty("ModuleType")]
        public string ModuleType { get; }

        [ContextProperty("URL")]
        public string URL { get; }

        [ContextProperty("ExtensionName")]
        public string ExtensionName { get; }

        [ContextProperty("ObjectID")]
        public GuidWrapper ObjectID { get; }

        [ContextProperty("PropertyID")]
        public GuidWrapper PropertyID { get; }
        
        [ContextProperty("ExtId")]
        public int ExtId { get; }

        [ContextProperty("Version")]
        public string Version { get; }
    }
}
