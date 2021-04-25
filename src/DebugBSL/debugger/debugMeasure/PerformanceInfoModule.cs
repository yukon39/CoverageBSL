using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugMeasure
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugMeasure")]
    public class PerformanceInfoModule
    {
        [XmlElement(ElementName = "moduleID")]
        public BSLModuleIdInternal ModuleID;

        [XmlElement(ElementName = "lineInfo")]
        public readonly List<PerformanceInfoLine> LineInfo = new();
    }
}
