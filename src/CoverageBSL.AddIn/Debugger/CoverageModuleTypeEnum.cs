using com.github.yukon39.DebugBSL.debugger.debugBaseData;

#if NET48
using ScriptEngine;
#else
using OneScript.Contexts.Enums;
#endif

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [EnumerationType("CoverageModuleType", "ТипМодуляПокрытия")]
    public enum CoverageModuleTypeEnum
    {
#if NET48
        [EnumItem("ConfigModule", "МодульКонфигурации")]
#else
        [EnumValue("ConfigModule", "МодульКонфигурации")]
#endif
        ConfigModule = BSLModuleType.ConfigModule,

#if NET48
        [EnumItem("SystemFormModule", "МодульСистемнойФормы")]
#else
        [EnumValue("SystemFormModule", "МодульСистемнойФормы")]
#endif
        SystemFormModule = BSLModuleType.SystemFormModule,

#if NET48
        [EnumItem("SystemModule", "МодульСистемы")]
#else
        [EnumValue("SystemModule", "МодульСистемы")]
#endif
        SystemModule = BSLModuleType.SystemModule,

#if NET48
        [EnumItem("ExtMDModule", "ВнешнийМодуль")]
#else
        [EnumValue("ExtMDModule", "ВнешнийМодуль")]
#endif
        ExtMDModule = BSLModuleType.ExtMDModule,

#if NET48
        [EnumItem("ExtensionModule", "МодульРасширения")]
#else
        [EnumValue("ExtensionModule", "МодульРасширения")]
#endif
        ExtensionModule = BSLModuleType.ExtensionModule
    }
}
