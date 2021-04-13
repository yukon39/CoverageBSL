using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugBreakpoints
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugBreakpoints")]
    public class BPWorkspaceInternal
    {
        [XmlElement(ElementName = "moduleBPInfo")]
        public readonly List<ModuleBPInfoInternal> ModuleBPInfo = new();
    }
}
