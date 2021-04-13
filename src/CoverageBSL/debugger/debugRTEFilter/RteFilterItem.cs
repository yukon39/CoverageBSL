using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugRTEFilter
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRTEFilter")]
    public class RteFilterItem
    {
        [XmlElement(ElementName = "include")]
        public bool Include;

        [XmlElement(ElementName = "str")]
        public string Str;
    }
}
