using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using com.github.yukon39.CoverageBSL.httpDebug;
using NUnit.Framework;

namespace CoverageBSL.Tests.debugger.debugRDBGRequestResponse
{
    class RDBGGetDbgAllTargetStatesTest
    {
        [Test]
        public void TestResponseDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugRDBGRequestResponse",
                "RDBGGetDbgAllTargetStatesResponseTest.xml");

            // When
            var request = HTTPDebugSerializer.Deserialize<RDBGGetDbgAllTargetStatesResponse>(xmlString);

            // Then
            Assert.AreEqual(2, request.Item.Count);

            var targetStateInfo = request.Item[0];
            Assert.AreEqual("DefAlias", targetStateInfo.TargetID.InfoBaseAlias);
            Assert.AreEqual(DbgTargetState.Worked, targetStateInfo.State);
        }
    }
}
