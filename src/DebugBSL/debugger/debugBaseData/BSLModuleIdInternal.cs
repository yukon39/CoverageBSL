using System;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    public class BSLModuleIdInternal
    {
        [XmlElement(ElementName = "type")]
        public BSLModuleType Type { get; set; }

        [XmlElement(ElementName = "url")]
        public string URL { get; set; } = string.Empty;

        [XmlElement(ElementName = "extensionName")]
        public string ExtensionName { get; set; } = string.Empty;

        [XmlElement(ElementName = "objectID")]
        public Guid ObjectID { get; set; }

        [XmlElement(ElementName = "propertyID")]
        public Guid PropertyID { get; set; }

        [XmlElement(ElementName = "extId")]
        public int ExtId { get; set; } = 0;

        [XmlElement(ElementName = "version")]
        public string Version { get; set; } = string.Empty;
    }
}
