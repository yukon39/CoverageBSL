using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    public class DebugTargetIdStr
    {
        [XmlElement(ElementName = "value")]
        public byte[] Value;
    }
}
