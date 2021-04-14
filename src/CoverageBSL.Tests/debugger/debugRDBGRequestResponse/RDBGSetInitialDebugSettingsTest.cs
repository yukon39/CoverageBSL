using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using com.github.yukon39.CoverageBSL.httpDebug;
using NUnit.Framework;
using System;

namespace CoverageBSL.Tests.debugger.debugRDBGRequestResponse
{
    class RDBGSetInitialDebugSettingsTest
    {

        [Test]
        public void TestRequestSerialization()
        {
            // Given
            var request = new RDBGSetInitialDebugSettingsRequest
            {
                InfoBaseAlias = "DefAlias",
                IdOfDebuggerUI = Guid.Parse("dbe7b1e9-9786-4a25-8da8-304684fa2ce3"),
                Data = new()
                {
                    RTEProcessing = new()
                    {
                        StopOnErrors = true,
                        AnalyzeErrorStr = true
                    }
                }
            };

            // When
            var xmlString = HTTPDebugSerializer.Serialize(request, null);
            Console.Write(xmlString);

            // Then
            var o = HTTPDebugSerializer.Deserialize<RDBGSetInitialDebugSettingsRequest>(xmlString, null);
            Assert.AreEqual(request.InfoBaseAlias, o.InfoBaseAlias);
            Assert.AreEqual(request.IdOfDebuggerUI, o.IdOfDebuggerUI);
            Assert.AreEqual(request.Data.RTEProcessing.StopOnErrors, o.Data.RTEProcessing.StopOnErrors);
            Assert.AreEqual(request.Data.RTEProcessing.AnalyzeErrorStr, o.Data.RTEProcessing.AnalyzeErrorStr);
        }

        [Test]
        public void TestRequestDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugRDBGRequestResponse", "RDBGAttachDetachDebugTargetsRequest.xml");

            // When
            var request = HTTPDebugSerializer.Deserialize<RDBGSetInitialDebugSettingsRequest>(xmlString, null);

            // Then
            Assert.AreEqual(request.InfoBaseAlias, "DefAlias");
            Assert.AreEqual(request.IdOfDebuggerUI, Guid.Parse("dbe7b1e9-9786-4a25-8da8-304684fa2ce3"));
        }

    }
}
