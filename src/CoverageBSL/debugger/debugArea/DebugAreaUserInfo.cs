using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugArea
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugArea")]
    public class DebugAreaUserInfo
    {
        [XmlElement(ElementName = "name")]
        public string Name;

        [XmlElement(ElementName = "use")]
        public bool Use;
    }
}
