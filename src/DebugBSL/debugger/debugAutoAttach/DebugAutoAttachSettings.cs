using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugAutoAttach
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugAutoAttach")]
    public class DebugAutoAttachSettings
    {

        [XmlElement(ElementName = "targetType")]
        public readonly List<DebugTargetType> TargetType = new List<DebugTargetType>();

        [XmlElement(ElementName = "areaName")]
        public readonly List<string> AreaName = new List<string>();
    }
}
