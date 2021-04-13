using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public class RDBGAttachDebugUIResponse : IRDBGResponse
    {
        [XmlElement(ElementName = "result")]
        public AttachDebugUIResult Result;
    }
}
