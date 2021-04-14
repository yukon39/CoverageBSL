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
            var XmlString = HTTPDebugSerializer.Serialize(Request, HTTPDebugSerializer.RootAttributeRequest);
            Console.Write(XmlString);

            // Then
            var Object = HTTPDebugSerializer.Deserialize<RDBGAttachDetachDebugTargetsRequest>(XmlString,
                    HTTPDebugSerializer.RootAttributeRequest);
            Assert.AreEqual(Request.InfoBaseAlias, Object.InfoBaseAlias);
            Assert.AreEqual(Request.IdOfDebuggerUI, Object.IdOfDebuggerUI);
            Assert.AreEqual(Request.Attach, Object.Attach);
            Assert.AreEqual(Request.ID[0].ID, Object.ID[0].ID);
        }

        [Test]
        public void TestRequestDeserialization()
        {
            // Given
            var XmlFile = Path.Join(TestContext.CurrentContext.TestDirectory,
                    "debugger", "debugRDBGRequestResponse", "RDBGAttachDetachDebugTargetsRequest.xml");
            var XmlString = File.ReadAllText(XmlFile);

            // When
            var Request = HTTPDebugSerializer.Deserialize<RDBGAttachDetachDebugTargetsRequest>(XmlString,
                HTTPDebugSerializer.RootAttributeRequest);

            // Then
            Assert.AreEqual(Request.InfoBaseAlias, "DefAlias");
            Assert.AreEqual(Request.IdOfDebuggerUI, Guid.Parse("dbe7b1e9-9786-4a25-8da8-304684fa2ce3"));
            Assert.True(Request.Attach);
            Assert.AreEqual(Request.ID[0].ID, Guid.Parse("f8849103-dbcd-4984-905d-28059c33a720"));
        }

    }
}
