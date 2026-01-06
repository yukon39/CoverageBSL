using NUnit.Framework;
using System;

namespace com.github.yukon39.DebugBSL.Client.Data.Tests
{
    class RequestParametersTests
    {
        [Test]
        public void TestURLWithCommand()
        {
            // Given
            var rootUrl = new Uri("http://localhost:1550");
            var parameters = new RequestParameters("testCommand");

            // When
            var requestUrl = parameters.RequestUrl(rootUrl);

            // Then
            Assert.That(requestUrl.ToString(), Is.EqualTo("http://localhost:1550/e1crdbg/rdbg?cmd=testCommand"));
        }

        [Test]
        public void TestURLWithResource()
        {
            // Given
            var rootUrl = new Uri("http://localhost:1550");
            var parameters = new RequestParameters("testCommand", "testRsc");

            // When
            var requestUrl = parameters.RequestUrl(rootUrl);

            // Then
            Assert.That(requestUrl.ToString(), Is.EqualTo("http://localhost:1550/e1crdbg/testRsc?cmd=testCommand"));
        }

        [Test]
        public void TestURLWithDebugID()
        {
            // Given
            var rootUrl = new Uri("http://localhost:1550");
            var debugId = Guid.NewGuid();
            var expectedUrl = string.Format("http://localhost:1550/e1crdbg/rdbg?cmd=testCommand&dbgui={0}", debugId);
            var parameters = new RequestParameters("testCommand");
            parameters.DebugID = debugId;

            // When
            var requestUrl = parameters.RequestUrl(rootUrl);

            // Then
            Assert.That(requestUrl.ToString(), Is.EqualTo(expectedUrl));
        }
    }
}
