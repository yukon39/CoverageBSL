using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugDBGUICommands;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using com.github.yukon39.CoverageBSL.httpDebug;
using NUnit.Framework;

namespace CoverageBSL.Tests.debugger.debugDBGUICommands
{
    class DBGUIExtCmdInfoQuitTest
    {
        [Test]
        public void TestDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugDBGUICommands", "DBGUIExtCmdInfoQuitTest.xml");

            // When
            var response = HTTPDebugSerializer.Deserialize<RDBGPingDebugUIResponse>(xmlString);

            // Then
            Assert.AreEqual(response.Result.Count, 1);
            Assert.IsInstanceOf<DBGUIExtCmdInfoQuit>(response.Result[0]);
            var infoStarted = (DBGUIExtCmdInfoQuit)response.Result[0];

            Assert.IsInstanceOf<DebugTargetId>(infoStarted.TargetID);
            Assert.AreEqual("DefAlias", infoStarted.TargetID.InfoBaseAlias);
        }
    }
}
