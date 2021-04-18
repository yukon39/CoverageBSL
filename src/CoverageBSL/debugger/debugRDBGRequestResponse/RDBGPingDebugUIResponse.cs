using com.github.yukon39.CoverageBSL.debugger.debugDBGUICommands;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse
{
    [XmlRoot(ElementName = "response", Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    [XmlInclude(typeof(DBGUIExtCmdInfoStarted))]
    [XmlInclude(typeof(DBGUIExtCmdInfoQuit))]
    [XmlInclude(typeof(DBGUIExtCmdInfoMeasure))]
    public class RDBGPingDebugUIResponse : IRDBGResponse
    {
        [XmlElement(ElementName = "result")]
        public readonly List<DBGUIExtCmdInfoBase> Result = new();
    }
}
