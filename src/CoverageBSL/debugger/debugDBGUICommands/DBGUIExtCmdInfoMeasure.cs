using com.github.yukon39.CoverageBSL.debugger.debugMeasure;
using System.Xml.Serialization;

namespace com.github.yukon39.CoverageBSL.debugger.debugDBGUICommands
{
    [XmlType(Namespace = "http://v8.1c.ru/8.3/debugger/debugDBGUICommands")]
    class DBGUIExtCmdInfoMeasure : DBGUIExtCmdInfoBase
    {

        [XmlElement(ElementName = "measureStr")]
        public char[] MeasureStr;

        [XmlElement(ElementName = "measure")]
        public PerformanceInfoMain Measure;

        public DBGUIExtCmdInfoMeasure() : base(DBGUIExtCmds.MeasureResultProcessing)
        {
        }
    }
}
