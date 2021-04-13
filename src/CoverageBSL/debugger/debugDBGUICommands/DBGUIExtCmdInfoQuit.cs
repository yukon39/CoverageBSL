using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugDBGUICommands
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugDBGUICommands")]
    class DBGUIExtCmdInfoQuit : DBGUIExtCmdInfoBase
    {
        public DBGUIExtCmdInfoQuit() : base(DBGUIExtCmds.TargetQuit)
        {
        }
    }
}
