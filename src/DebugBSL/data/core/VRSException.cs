using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.data.core
{
    [XmlRoot(ElementName = "exception", Namespace = "http://v8.1c.ru/8.2/virtual-resource-system")]
    [XmlType(TypeName = "Exception", Namespace = "http://v8.1c.ru/8.2/virtual-resource-system")]
    public class VRSException : GenericException
    {
        [XmlAttribute(AttributeName = "reason")]
        public int Reason;
    }
}
