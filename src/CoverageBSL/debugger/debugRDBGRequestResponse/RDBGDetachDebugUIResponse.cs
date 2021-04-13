using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse
{

    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    class RDBGDetachDebugUIResponse : IRDBGResponse
    {
        [XmlElement(ElementName = "result")]
        public bool Result { get; }
    }
}
