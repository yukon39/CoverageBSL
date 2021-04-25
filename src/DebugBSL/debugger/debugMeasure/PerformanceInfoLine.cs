using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugMeasure
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugMeasure")]
    public class PerformanceInfoLine
    {
        [XmlElement(ElementName = "LineNo")]
        public int LineNo;

        [XmlElement(ElementName = "Frequency")]
        public int Frequency;

        [XmlElement(ElementName = "Durability")]
        public long Durability;

        [XmlElement(ElementName = "PureDurability")]
        public long PureDurability;

        [XmlElement(ElementName = "ServerCallSignal")]
        public byte ServerCallSignal;
    }
}
