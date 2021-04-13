﻿using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using com.github.yukon39.CoverageBSL.debugger.debugBreakpoints;
using com.github.yukon39.CoverageBSL.debugger.debugRTEFilter;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugRDBGRequestResponse")]
    public class HTTPInitialDebugSettingsData
    {
        [XmlElement(ElementName = "inacessibleModuleID")]
        public List<BSLModuleIdInternal> InacessibleModuleID;

        [XmlElement(ElementName = "envStateVersion")]
        public Guid EnvStateVersion;

        [XmlElement(ElementName = "breakOnNextLine")]
        public bool BreakOnNextLine;

        [XmlElement(ElementName = "measureMode")]
        public Guid MeasureMode;

        [XmlElement(ElementName = "serverIndependentWorkTime")]
        public int ServerIndependentWorkTime;

        [XmlElement(ElementName = "bpWorkspace")]
        public BPWorkspaceInternal BPWorkspace;

        [XmlElement(ElementName = "bpVersion")]
        public Guid BPVersion;

        [XmlElement(ElementName = "rteProcessing")]
        public RteFilterStorage RTEProcessing;

        [XmlElement(ElementName = "rteProcVersion")]
        public Guid RTEProcVersion;

        [XmlElement(ElementName = "inacessibleModuleVersion")]
        public Guid InacessibleModuleVersion;
    }
}
