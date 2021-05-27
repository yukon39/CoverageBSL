using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugDBGUICommands
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugDBGUICommands")]
    public abstract class DBGUIExtCmdInfoBase
    {
        [XmlElement(ElementName = "cmdIDNum")]
        public int CmdIDNum;

        [XmlElement(ElementName = "cmdId")]
        public readonly DBGUIExtCmds CmdId;

        [XmlElement(ElementName = "targetIDStr")]
        public DebugTargetIdStr TargetIDStr;

        [XmlElement(ElementName = "targetID")]
        public DebugTargetId TargetID;

        [XmlElement(ElementName = "requestQueueID")]
        public string requestQueueID;

        public DBGUIExtCmdInfoBase() : this(DBGUIExtCmds.Unknown)
        {

        }

        public DBGUIExtCmdInfoBase(DBGUIExtCmds cmdId)
        {
            CmdId = cmdId;
            CmdIDNum = (int)cmdId;
        }
    }
}
