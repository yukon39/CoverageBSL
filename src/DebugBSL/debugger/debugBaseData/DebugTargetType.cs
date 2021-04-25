using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData", TypeName = "DebugTargetType")]
    public enum DebugTargetType
    {
        [XmlEnum(Name = "Unknown")]
        Unknown,

        [XmlEnum(Name = "Client")]
        Client,

        [XmlEnum(Name = "ManagedClient")]
        ManagedClient,

        [XmlEnum(Name = "WEBClient")]
        WEBClient,

        [XmlEnum(Name = "COMConnector")]
        COMConnector,

        [XmlEnum(Name = "Server")]
        Server,

        [XmlEnum(Name = "ServerEmulation")]
        ServerEmulation,

        [XmlEnum(Name = "WEBService")]
        WEBService,

        [XmlEnum(Name = "HTTPService")]
        HTTPService,

        [XmlEnum(Name = "OData")]
        OData,

        [XmlEnum(Name = "JOB")]
        JOB,

        [XmlEnum(Name = "JobFileMode")]
        JobFileMode,

        [XmlEnum(Name = "MobileClient")]
        MobileClient,

        [XmlEnum(Name = "MobileServer")]
        MobileServer,

        [XmlEnum(Name = "MobileJobFileMode")]
        MobileJobFileMode,

        [XmlEnum(Name = "MobileManagedClient")]
        MobileManagedClient
    }
}
