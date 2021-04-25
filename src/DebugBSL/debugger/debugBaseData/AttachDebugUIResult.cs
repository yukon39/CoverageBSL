using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    public enum AttachDebugUIResult
    {
        [XmlEnum(Name = "unknown")]
        Unknown,

        [XmlEnum(Name = "registered")]
        Registered,

        [XmlEnum(Name = "credentialsRequired")]
        CredentialsRequired,

        [XmlEnum(Name = "ibInDebug")]
        IBInDebug,

        [XmlEnum(Name = "notRegistered")]
        NotRegistered
    }
}
