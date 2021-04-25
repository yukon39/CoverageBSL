using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugArea
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugArea")]
    public class DebugAreaDSPairInfo
    {
        [XmlElement(ElementName = "name")]
        public string Name;

        [XmlElement(ElementName = "value")]
        public object Value;

        [XmlElement(ElementName = "use")]
        public bool Use;
    }
}
