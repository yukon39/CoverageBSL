using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugDBGUICommands
{
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
