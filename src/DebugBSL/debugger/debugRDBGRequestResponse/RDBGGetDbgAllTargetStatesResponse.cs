using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse
{
    [XmlRoot(ElementName = "response", Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public class RDBGGetDbgAllTargetStatesResponse : IRDBGResponse
    {
        [XmlElement(ElementName = "item")]
        public readonly List<DbgTargetStateInfo> Item = new List<DbgTargetStateInfo>();
    }
}
