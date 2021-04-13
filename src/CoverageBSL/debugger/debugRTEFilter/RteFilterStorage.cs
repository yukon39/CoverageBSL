using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugRTEFilter
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRTEFilter")]
    public class RteFilterStorage
    {
        [XmlElement(ElementName = "strTemplate")]
        public readonly List<RteFilterItem> StrTemplate = new();

        [XmlElement(ElementName = "stopOnErrors")]
        public bool StopOnErrors;

        [XmlElement(ElementName = "analyzeErrorStr")]
        public bool AnalyzeErrorStr;
    }
}
