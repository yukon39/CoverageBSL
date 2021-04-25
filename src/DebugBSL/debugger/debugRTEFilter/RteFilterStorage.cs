using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugRTEFilter
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRTEFilter")]
    public class RteFilterStorage
    {
        [XmlElement(ElementName = "strTemplate")]
        public List<RteFilterItem> StrTemplate { get; } = new List<RteFilterItem>();

        [XmlElement(ElementName = "stopOnErrors")]
        public bool StopOnErrors { get; set; }

        [XmlElement(ElementName = "analyzeErrorStr")]
        public bool AnalyzeErrorStr { get; set; }
    }
}
