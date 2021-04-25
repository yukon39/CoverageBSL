using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse
{
    [XmlRoot(ElementName = "request", Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public class RDBGAttachDebugUIRequest : RDbgBaseRequest, IRDBGRequest
    {
        [XmlElement(ElementName = "credentials")]
        public byte[] Credentials;

        [XmlElement(ElementName = "options")]
        public DebuggerOptions Options;
    }
}
