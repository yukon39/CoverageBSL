using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    public class DbgTargetStateInfo
    {
        [XmlElement(ElementName = "targetIDStr")]
        public DebugTargetIdStr TargetIDStr;

        [XmlElement(ElementName = "targetID")]
        public DebugTargetId TargetID;

        [XmlElement(ElementName = "stateNum")]
        public int StateNum;

        [XmlElement(ElementName = "state")]
        public DbgTargetState State;
    }
}
