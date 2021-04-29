using com.github.yukon39.DebugBSL;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugClientBSL.Tests
{
    class HTTPDebugClientTest
    {
        [Test]
        public void TestTestAsync()
        {
            // given
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var client = Create(messageHandler);

            // when
            Assert.DoesNotThrowAsync(async () => await client.TestAsync());

            // then
            var request = messageHandler.Dequeue();
            Assert.AreEqual("http://localhost/e1crdbg/rdbgTest?cmd=test", request.RequestUri.ToString());
        }

        [Test]
        public async Task TestApiVersionAsync()
        {
            // given
            var response = new MiscRDbgGetAPIVerResponse()
            {
                Version = "1.2.3.4"
            };

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var client = Create(messageHandler);

            // when
            var version = await client.ApiVersionAsync();

            // then
            Assert.AreEqual("1.2.3.4", version);

            var request = messageHandler.Dequeue();
            Assert.AreEqual("http://localhost/e1crdbg/rdbg?cmd=getRDbgAPIVer", request.RequestUri.ToString());
        }

        [Test]
        public void TestCreateSession()
        {
            // given
            var client = Create(new MockHttpMessageHandler());

            // when
            var session = client.CreateSession("testAlias");

            // than
            Assert.IsInstanceOf<IDebuggerClientSession>(session);
        }

        private static IDebuggerClient Create(MockHttpMessageHandler messageHandler)
        {
            var executor = messageHandler.CreateExecutor();
            return HTTPDebugClient.Build(executor);
        }
    }
}
