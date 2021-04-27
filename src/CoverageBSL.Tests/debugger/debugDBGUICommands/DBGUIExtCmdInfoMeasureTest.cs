using com.github.yukon39.DebugBSL;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugDBGUICommands;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;

namespace CoverageBSL.Tests.debugger.debugDBGUICommands
{
    class DBGUIExtCmdInfoMeasureTest
    {
        [Test]
        public void TestDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugDBGUICommands", "DBGUIExtCmdInfoMeasureTest.xml");

            // When
            var response = DebuggerXmlSerializer.Deserialize<RDBGPingDebugUIResponse>(xmlString);

            // Then
            Assert.AreEqual(response.Result.Count, 1);
            Assert.IsInstanceOf<DBGUIExtCmdInfoMeasure>(response.Result[0]);
            var infoMeasure = response.Result[0] as DBGUIExtCmdInfoMeasure;

            Assert.IsInstanceOf<DebugTargetId>(infoMeasure.TargetID);
            Assert.AreEqual("DefAlias", infoMeasure.TargetID.InfoBaseAlias);
            Assert.AreEqual("DefAlias", infoMeasure.Measure.TargetID.InfoBaseAlias);
            Assert.Greater(infoMeasure.Measure.TotalDurability, 0);
            Assert.Greater(infoMeasure.Measure.ModuleData.Count, 0);

            var moduleData = infoMeasure.Measure.ModuleData[0];
            Assert.IsInstanceOf<BSLModuleIdInternal>(moduleData.ModuleID);
            Assert.Greater(moduleData.LineInfo.Count, 0);

            var lineInfo = moduleData.LineInfo[0];
            Assert.Greater(lineInfo.LineNo, 0);
        }
    }
}
