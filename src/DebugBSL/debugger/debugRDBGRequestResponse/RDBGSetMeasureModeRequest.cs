using System;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse
{
    [XmlRoot(ElementName = "response", Namespace = "http://v8.1c.ru/8.3/debugger/debugBaseData")]
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public class RDBGSetMeasureModeRequest : RDbgBaseRequest, IRDBGRequest
    {
        [XmlElement(ElementName = "measureModeSeanceID", IsNullable = false)]
        public Guid MeasureModeSeanceID { get; set; }

        public bool ShouldSerializeMeasureModeSeanceID() => MeasureModeSeanceID != Guid.Empty;
    }
}
