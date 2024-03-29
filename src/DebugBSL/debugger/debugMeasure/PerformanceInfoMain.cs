﻿using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugMeasure
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugMeasure")]
    public class PerformanceInfoMain
    {
        [XmlElement(ElementName = "targetID")]
        public DebugTargetId TargetID;

        [XmlElement(ElementName = "totalDurability")]
        public long TotalDurability;

        [XmlElement(ElementName = "totalIndepServerWorkTime")]
        public double TotalIndepServerWorkTime;

        [XmlElement(ElementName = "performanceFrequency")]
        public int PerformanceFrequency;

        [XmlElement(ElementName = "moduleData")]
        public readonly List<PerformanceInfoModule> ModuleData = new List<PerformanceInfoModule>();

        [XmlElement(ElementName = "sessionID")]
        public Guid SessionID;
    }
}
