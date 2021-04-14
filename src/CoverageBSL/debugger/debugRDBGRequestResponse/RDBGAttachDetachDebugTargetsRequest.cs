using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse
{
    [XmlRoot(ElementName = "request", Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public class RDBGAttachDetachDebugTargetsRequest : RDbgBaseRequest, IRDBGRequest
    {
        [XmlElement(ElementName = "attach")]
        public bool Attach;

        [XmlElement(ElementName = "id")]
        public readonly List<DebugTargetIdLight> ID = new();
    }
}
