using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugMeasure
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugMeasure")]
    public class PerformanceInfoLine
    {
        [XmlElement(ElementName = "lineNo")]
        public int LineNo { get; set; }

        [XmlElement(ElementName = "frequency")]
        public int Frequency { get; set; }

        [XmlElement(ElementName = "durability")]
        public long Durability { get; set; }

        [XmlElement(ElementName = "pureDurability")]
        public long PureDurability { get; set; }

        [XmlElement(ElementName = "serverCallSignal")]
        public byte ServerCallSignal { get; set; }
    }
}
