using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugAutoAttach
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugAutoAttach")]
    public class DebugAutoAttachSettings
    {

        [XmlElement(ElementName = "targetType")]
        public readonly List<DebugTargetType> TargetType = new();

        [XmlElement(ElementName = "areaName")]
        public readonly List<string> AreaName = new();
    }
}
