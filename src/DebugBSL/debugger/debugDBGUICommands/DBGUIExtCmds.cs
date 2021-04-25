using System.Xml.Serialization;

namespace com.github.yukon39.DebugBSL.debugger.debugDBGUICommands
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugDBGUICommands")]
    public enum DBGUIExtCmds
    {
        [XmlEnum(Name = "unknown")]
        Unknown,

        [XmlEnum(Name = "targetStarted")]
        TargetStarted,

        [XmlEnum(Name = "targetQuit")]
        TargetQuit,

        [XmlEnum(Name = "correctedBP")]
        CorrectedBP,

        [XmlEnum(Name = "rteProcessing")]
        RTEProcessing,

        [XmlEnum(Name = "rteOnBPConditionProcessing")]
        RTEOnBPConditionProcessing,

        [XmlEnum(Name = "measureResultProcessing")]
        MeasureResultProcessing,

        [XmlEnum(Name = "callStackFormed")]
        CallStackFormed,

        [XmlEnum(Name = "exprEvaluated")]
        ExprEvaluated,

        [XmlEnum(Name = "unvalueModifiedknown")]
        ValueModified,

        [XmlEnum(Name = "errorViewInfo")]
        ErrorViewInfo,

        [XmlEnum(Name = "ForegroundHelperSet")]
        ForegroundHelperSet,

        [XmlEnum(Name = "ForegroundHelperRequest")]
        ForegroundHelperRequest,

        [XmlEnum(Name = "ForegroundHelperProcess")]
        ForegroundHelperProcess
    }
}
