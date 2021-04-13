using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugDBGUICommands
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugDBGUICommands")]
    class DBGUIExtCmdInfoStarted : DBGUIExtCmdInfoBase
    {
        public DBGUIExtCmdInfoStarted() : base(DBGUIExtCmds.TargetStarted)
        {
        }
    }
}
