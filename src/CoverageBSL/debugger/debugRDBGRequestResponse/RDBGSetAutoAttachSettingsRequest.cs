using com.github.yukon39.CoverageBSL.debugger.debugAutoAttach;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse
{
    [XmlRoot(ElementName = "request", Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public class RDBGSetAutoAttachSettingsRequest : RDbgBaseRequest, IRDBGRequest
    {
        [XmlElement(ElementName = "autoAttachSettings")]
        public DebugAutoAttachSettings AutoAttachSettings;
    }
}
