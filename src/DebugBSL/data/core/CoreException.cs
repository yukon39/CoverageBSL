using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.data.core
{
    [XmlRoot(ElementName = "exception", Namespace = "http://v8.1c.ru/8.2/virtual-resource-system")]
    [XmlType(TypeName = "Exception", Namespace = "http://v8.1c.ru/8.1/data/core")]
    public class CoreException : GenericException
    {
        [XmlElement(ElementName = "data")]
        public byte[] Data;
    }
}
