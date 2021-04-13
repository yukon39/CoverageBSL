using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    public enum IsServerInfoBase
    {
        [XmlEnum(Name = "undefined")]
        Undefined,

        [XmlEnum(Name = "true")]
        True,

        [XmlEnum(Name = "false")]
        False
    }
}
