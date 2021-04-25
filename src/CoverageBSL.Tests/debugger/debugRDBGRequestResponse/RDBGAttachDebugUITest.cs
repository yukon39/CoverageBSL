using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using com.github.yukon39.CoverageBSL.httpDebug;
using NUnit.Framework;
using System;

namespace CoverageBSL.Tests.debugger.debugRDBGRequestResponse
{
    class RDBGAttachDebugUITest
    {

        [Test]
        public void TestRequestSerialization()
        {
            // Given
            var request = new RDBGAttachDebugUIRequest
            {
                InfoBaseAlias = "DefAlias",
                IdOfDebuggerUI = Guid.Parse("dbe7b1e9-9786-4a25-8da8-304684fa2ce3"),
                Options = new DebuggerOptions
                {
                    ForegroundAbility = true
                }
            };

            // When
            var xmlString = HTTPDebugSerializer.Serialize(request);
            Console.Write(xmlString);

            // Then
            var xmlRequest = HTTPDebugSerializer.Deserialize<RDBGAttachDebugUIRequest>(xmlString);
            Assert.AreEqual(request.InfoBaseAlias, xmlRequest.InfoBaseAlias);
            Assert.AreEqual(request.IdOfDebuggerUI, xmlRequest.IdOfDebuggerUI);
            Assert.AreEqual(request.Options.ForegroundAbility, xmlRequest.Options.ForegroundAbility);
        }

        [Test]
        public void TestRequestDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugRDBGRequestResponse", "RDBGAttachDebugUIRequestTest.xml");

            // When
            var request = HTTPDebugSerializer.Deserialize<RDBGAttachDebugUIRequest>(xmlString);

            // Then
            Assert.AreEqual(request.InfoBaseAlias, "DefAlias");
            Assert.AreEqual(request.IdOfDebuggerUI, Guid.Parse("dbe7b1e9-9786-4a25-8da8-304684fa2ce3"));
            Assert.True(request.Options.ForegroundAbility);
        }

        [Test]
        public void TestResponseSerialization()
        {
            // Given
            var response = new RDBGAttachDebugUIResponse()
            {
                Result = AttachDebugUIResult.Registered
            };

            // When
            var xmlString = HTTPDebugSerializer.Serialize(response);
            Console.Write(xmlString);

            // Then
            var xmlResponse = HTTPDebugSerializer.Deserialize<RDBGAttachDebugUIResponse>(xmlString);
            Assert.AreEqual(response.Result, xmlResponse.Result);
        }

        [Test]
        public void TestResponseDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("debugger", "debugRDBGRequestResponse",
                    "RDBGAttachDebugUIResponseTest.xml");

            // When
            var request = HTTPDebugSerializer.Deserialize<RDBGAttachDebugUIResponse>(xmlString);

            // Then
            Assert.AreEqual(request.Result, AttachDebugUIResult.Registered);
        }
    }
}
