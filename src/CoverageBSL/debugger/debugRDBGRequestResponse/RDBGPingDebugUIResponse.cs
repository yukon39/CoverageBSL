using com.github.yukon39.CoverageBSL.debugger.debugDBGUICommands;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    class RDBGPingDebugUIResponse : IRDBGResponse
    {
        [XmlElement(ElementName = "result")]
        public readonly List<DBGUIExtCmdInfoBase> Result = new();
    }
}
