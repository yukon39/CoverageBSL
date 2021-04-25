using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugDBGUICommands
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugDBGUICommands")]
    public class DBGUIExtCmdInfoQuit : DBGUIExtCmdInfoBase
    {
        public DBGUIExtCmdInfoQuit() : base(DBGUIExtCmds.TargetQuit)
        {
        }
    }
}
