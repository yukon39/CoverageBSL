using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    public enum BSLModuleType
    {
        [XmlEnum(Name = "ConfigModule")]
        ConfigModule,

        [XmlEnum(Name = "SystemFormModule")]
        SystemFormModule,

        [XmlEnum(Name = "SystemModule")]
        SystemModule,

        [XmlEnum(Name = "ExtMDModule")]
        ExtMDModule,

        [XmlEnum(Name = "ExtensionModule")]
        ExtensionModule
    }
}
