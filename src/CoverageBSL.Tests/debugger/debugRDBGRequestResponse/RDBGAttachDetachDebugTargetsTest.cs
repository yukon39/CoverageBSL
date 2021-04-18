using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using com.github.yukon39.CoverageBSL.httpDebug;
using NUnit.Framework;
using System;
using System.IO;

namespace CoverageBSL.Tests.debugger.debugRDBGRequestResponse
{
    class RDBGAttachDetachDebugTargetsTest
    {
        [Test]
        public void TestRequestSerialization()
        {
            // Given
            var Request = new RDBGAttachDetachDebugTargetsRequest
            {
                InfoBaseAlias = "DefAlias",
                IdOfDebuggerUI = Guid.Parse("dbe7b1e9-9786-4a25-8da8-304684fa2ce3"),
                Attach = true
            };
            Request.ID.Add(new() { ID = Guid.Parse("f8849103-dbcd-4984-905d-28059c33a720") });

            // When
            var XmlString = HTTPDebugSerializer.Serialize(Request);
            Console.Write(XmlString);

            // Then
            var Object = HTTPDebugSerializer.Deserialize<RDBGAttachDetachDebugTargetsRequest>(XmlString);
            Assert.AreEqual(Request.InfoBaseAlias, Object.InfoBaseAlias);
            Assert.AreEqual(Request.IdOfDebuggerUI, Object.IdOfDebuggerUI);
            Assert.AreEqual(Request.Attach, Object.Attach);
            Assert.AreEqual(Request.ID[0].ID, Object.ID[0].ID);
        }

        [Test]
        public void TestRequestDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugRDBGRequestResponse", 
                "RDBGAttachDetachDebugTargetsRequest.xml");

            // When
            var request = HTTPDebugSerializer.Deserialize<RDBGAttachDetachDebugTargetsRequest>(xmlString);

            // Then
            Assert.AreEqual(request.InfoBaseAlias, "DefAlias");
            Assert.AreEqual(request.IdOfDebuggerUI, Guid.Parse("dbe7b1e9-9786-4a25-8da8-304684fa2ce3"));
            Assert.True(request.Attach);
            Assert.AreEqual(request.ID[0].ID, Guid.Parse("f8849103-dbcd-4984-905d-28059c33a720"));
        }
    }
}
