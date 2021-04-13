using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugBreakpoints
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBreakpoints")]
    public class BreakpointInfo
    {
        [XmlElement(ElementName = "line")]
        public int Line;

        [XmlElement(ElementName = "isActive")]
        public bool IsActive;

        [XmlElement(ElementName = "condition")]
        public string Condition;

        [XmlElement(ElementName = "temp")]
        public bool Temp;

        [XmlElement(ElementName = "user")]
        public bool User;
    }
}
