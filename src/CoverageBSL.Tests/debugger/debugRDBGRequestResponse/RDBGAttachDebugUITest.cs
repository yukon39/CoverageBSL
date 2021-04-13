using com.github.yukon39.CoverageBSL.debugger.debugBaseData;
using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using com.github.yukon39.CoverageBSL.httpDebug;
using NUnit.Framework;
using System;
using System.IO;

namespace CoverageBSL.Tests.debugger.debugRDBGRequestResponse
{
    class RDBGAttachDebugUITest
    {

        [Test]
        public void TestRequestSerialization()
        {
            // Given
            var Request = new RDBGAttachDebugUIRequest
            {
                InfoBaseAlias = "DefAlias",
                IdOfDebuggerUI = Guid.Parse("dbe7b1e9-9786-4a25-8da8-304684fa2ce3"),
                Options = new DebuggerOptions
                {
                    ForegroundAbility = true
                }
            };

            // When
            var XmlString = HTTPDebugSerializer.Serialize(Request, HTTPDebugSerializer.RootAttributeRequest);
            Console.Write(XmlString);

            // Then

            var Object = HTTPDebugSerializer.Deserialize<RDBGAttachDebugUIRequest>(XmlString, HTTPDebugSerializer.RootAttributeRequest);
            Assert.AreEqual(Request.InfoBaseAlias, Object.InfoBaseAlias);
            Assert.AreEqual(Request.IdOfDebuggerUI, Object.IdOfDebuggerUI);
            Assert.AreEqual(Request.Options.ForegroundAbility, Object.Options.ForegroundAbility);
        }

        [Test]
        public void TestRequestDeserialization()
        {
            // Given
            var XmlFile = Path.Join(TestContext.CurrentContext.TestDirectory,
                    "debugger", "debugRDBGRequestResponse", "RDBGAttachDebugUIRequestTest.xml");
            var XmlString = File.ReadAllText(XmlFile);

            // When
            var Request = HTTPDebugSerializer.Deserialize<RDBGAttachDebugUIRequest>(XmlString, HTTPDebugSerializer.RootAttributeRequest);

            // Then
            Assert.AreEqual(Request.InfoBaseAlias, "DefAlias");
            Assert.AreEqual(Request.IdOfDebuggerUI, Guid.Parse("dbe7b1e9-9786-4a25-8da8-304684fa2ce3"));
            Assert.True(Request.Options.ForegroundAbility);
        }

        [Test]
        public void TestResponseSerialization()
        {
            // Given
            var Response = new RDBGAttachDebugUIResponse()
            {
                Result = AttachDebugUIResult.Registered
            };

            // When
            var XmlString = HTTPDebugSerializer.Serialize(Response, HTTPDebugSerializer.RootAttributeResponse);
            Console.Write(XmlString);

            // Then
            var Object = HTTPDebugSerializer.Deserialize<RDBGAttachDebugUIResponse>(XmlString, HTTPDebugSerializer.RootAttributeResponse);
            Assert.AreEqual(Response.Result, Object.Result);
        }

        [Test]
        public void TestResponseDeserialization()
        {
            // Given
            var XmlFile = Path.Join(TestContext.CurrentContext.TestDirectory,
                    "debugger", "debugRDBGRequestResponse", "RDBGAttachDebugUIResponseTest.xml");
            var XmlString = File.ReadAllText(XmlFile);

            // When
            var Request = HTTPDebugSerializer.Deserialize<RDBGAttachDebugUIResponse>(XmlString, HTTPDebugSerializer.RootAttributeResponse);

            // Then
            Assert.AreEqual(Request.Result, AttachDebugUIResult.Registered);
        }
    }
}
