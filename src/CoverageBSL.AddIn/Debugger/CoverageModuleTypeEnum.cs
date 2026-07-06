using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using OneScript.Contexts.Enums;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [EnumerationType("CoverageModuleType", "ТипМодуляПокрытия")]
    public enum CoverageModuleTypeEnum
    {
        [EnumValue("ConfigModule", "МодульКонфигурации")]
        ConfigModule = BSLModuleType.ConfigModule,

        [EnumValue("SystemFormModule", "МодульСистемнойФормы")]
        SystemFormModule = BSLModuleType.SystemFormModule,

        [EnumValue("SystemModule", "МодульСистемы")]
        SystemModule = BSLModuleType.SystemModule,

        [EnumValue("ExtMDModule", "ВнешнийМодуль")]
        ExtMDModule = BSLModuleType.ExtMDModule,

        [EnumValue("ExtensionModule", "МодульРасширения")]
        ExtensionModule = BSLModuleType.ExtensionModule
    }
}
