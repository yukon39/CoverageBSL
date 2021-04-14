using System;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.data.core
{
    public abstract class GenericException
    {
        [XmlAttribute(AttributeName = "clsid")]
        public Guid CLSID;

        [XmlAttribute(AttributeName = "encoded")]
        public bool Encoded;

        [XmlElement(ElementName = "descr")]
        public string Description;

        [XmlElement(ElementName = "inner")]
        public GenericException Inner;

        [XmlElement(ElementName = "category")]
        public string Category;

        [XmlElement(ElementName = "uiHelperUUID")]
        public Guid UIHelperUUID;

        [XmlElement(ElementName = "creationStack")]
        public string CreationStack;
    }
}
