using ScriptEngine;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    [EnumerationType("AttachDebugUIResult", "РезультатПодключенияОтладки")]
    public enum AttachDebugUIResult
    {
        [XmlEnum(Name = "unknown")]
        [EnumItem("Unknown", "Неизвестно")]
        Unknown,

        [XmlEnum(Name = "registered")]
        [EnumItem("Registered", "Зарегистровано")]
        Registered,

        [XmlEnum(Name = "credentialsRequired")]
        [EnumItem("CredentialsRequired", "ТребуетсяАвторизация")]
        CredentialsRequired,

        [XmlEnum(Name = "ibInDebug")]
        [EnumItem("IBInDebug", "БазаВРежимеОтладки")]
        IBInDebug,

        [XmlEnum(Name = "notRegistered")]
        [EnumItem("NotRegistered", "Незарегистровано")]
        NotRegistered
    }
}
