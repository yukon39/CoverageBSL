using System;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    public class BSLModuleIdInternal
    {
        [XmlElement(ElementName = "type")]
        public BSLModuleType Type;

        [XmlElement(ElementName = "url")]
        public string URL;

        [XmlElement(ElementName = "extensionName")]
        public string ExtensionName;

        [XmlElement(ElementName = "objectID")]
        public Guid ObjectID;

        [XmlElement(ElementName = "propertyID")]
        public Guid PropertyID;

        [XmlElement(ElementName = "extId")]
        public int ExtId;

        [XmlElement(ElementName = "version")]
        public string Version;
    }
}
