using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugBreakpoints;
using com.github.yukon39.DebugBSL.debugger.debugRTEFilter;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public class HTTPInitialDebugSettingsData
    {
        [XmlElement(ElementName = "inacessibleModuleID")]
        public List<BSLModuleIdInternal> InacessibleModuleID { get; } = new List<BSLModuleIdInternal>();

        [XmlElement(ElementName = "envStateVersion")]
        public Guid EnvStateVersion { get; set; }

        public bool ShouldSerializeEnvStateVersion() => EnvStateVersion != Guid.Empty;

        [XmlElement(ElementName = "breakOnNextLine")]
        public bool BreakOnNextLine { get; set; }

        [XmlElement(ElementName = "measureMode")]
        public Guid MeasureMode { get; set; }

        public bool ShouldSerializeMeasureMode() => MeasureMode != Guid.Empty;

        [XmlElement(ElementName = "serverIndependentWorkTime")]
        public int ServerIndependentWorkTime { get; set; }

        public bool ShouldSerializeServerIndependentWorkTime() => ServerIndependentWorkTime != 0;

        [XmlElement(ElementName = "bpWorkspace")]
        public BPWorkspaceInternal BPWorkspace { get; set; }

        [XmlElement(ElementName = "bpVersion")]
        public Guid BPVersion { get; set; }

        public bool ShouldSerializeBPVersion() => BPVersion != Guid.Empty;

        [XmlElement(ElementName = "rteProcessing")]
        public RteFilterStorage RTEProcessing { get; set; }

        [XmlElement(ElementName = "rteProcVersion")]
        public Guid RTEProcVersion { get; set; }

        public bool ShouldSerializeRTEProcVersion() => RTEProcVersion != Guid.Empty;

        [XmlElement(ElementName = "inacessibleModuleVersion")]
        public Guid InacessibleModuleVersion { get; set; }

        public bool ShouldSerializeInacessibleModuleVersion() => InacessibleModuleVersion != Guid.Empty;
    }
}
