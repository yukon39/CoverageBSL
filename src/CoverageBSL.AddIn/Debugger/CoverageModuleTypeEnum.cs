using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using ScriptEngine;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [EnumerationType("CoverageModuleType", "ТипМодуляПокрытия")]
    public enum CoverageModuleTypeEnum
    {
        [EnumItem("ConfigModule", "МодульКонфигурации")]
        ConfigModule = BSLModuleType.ConfigModule,

        [EnumItem("SystemFormModule", "МодульСистемнойФормы")]
        SystemFormModule = BSLModuleType.SystemFormModule,

        [EnumItem("SystemModule", "МодульСистемы")]
        SystemModule = BSLModuleType.SystemModule,

        [EnumItem("ExtMDModule", "ВнешнийМодуль")]
        ExtMDModule = BSLModuleType.ExtMDModule,

        [EnumItem("ExtensionModule", "МодульРасширения")]
        ExtensionModule = BSLModuleType.ExtensionModule
    }
}
