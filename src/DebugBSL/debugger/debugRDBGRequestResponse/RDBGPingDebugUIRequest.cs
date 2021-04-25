using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse
{
    [XmlRoot(ElementName = "request", Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public class RDBGPingDebugUIRequest : RDbgBaseRequest, IRDBGRequest
    {
        [XmlElement(ElementName = "saveDataAsString")]
        public bool SaveDataAsString;
    }
}
