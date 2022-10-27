using com.github.yukon39.DebugBSL.debugger.debugBaseData;

#if NET5_0_OR_GREATER
using OneScript.Contexts.Enums;
#else
using ScriptEngine;
#endif

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [EnumerationType("CoverageModuleType", "ТипМодуляПокрытия")]
    public enum CoverageModuleTypeEnum
    {
#if NET5_0_OR_GREATER
        [EnumValue("ConfigModule", "МодульКонфигурации")]
#else
        [EnumItem("ConfigModule", "МодульКонфигурации")]
#endif
        ConfigModule = BSLModuleType.ConfigModule,

#if NET5_0_OR_GREATER
        [EnumValue("SystemFormModule", "МодульСистемнойФормы")]
#else
        [EnumItem("SystemFormModule", "МодульСистемнойФормы")]
#endif
        SystemFormModule = BSLModuleType.SystemFormModule,

#if NET5_0_OR_GREATER
        [EnumValue("SystemModule", "МодульСистемы")]
#else
        [EnumItem("SystemModule", "МодульСистемы")]
#endif
        SystemModule = BSLModuleType.SystemModule,

#if NET5_0_OR_GREATER
        [EnumValue("ExtMDModule", "ВнешнийМодуль")]
#else
        [EnumItem("ExtMDModule", "ВнешнийМодуль")]
#endif
        ExtMDModule = BSLModuleType.ExtMDModule,

#if NET5_0_OR_GREATER
        [EnumValue("ExtensionModule", "МодульРасширения")]
#else
        [EnumItem("ExtensionModule", "МодульРасширения")]
#endif
        ExtensionModule = BSLModuleType.ExtensionModule
    }
}
