using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugBreakpoints
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBreakpoints")]
    public class ModuleBPInfoInternal
    {
        [XmlElement(ElementName = "id")]
        public BSLModuleIdInternal ID;

        [XmlElement(ElementName = "bpInfo")]
        public readonly List<BreakpointInfo> BPInfo = new();
    }
}
