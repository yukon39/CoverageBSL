using System;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugBaseData
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    public class DebugTargetIdLight
    {
        [XmlElement(ElementName = "id")]
        public Guid ID;
    }
}
