using com.github.yukon39.DebugBSL.Client.Data;
using com.github.yukon39.DebugBSL.Client.Tests;
using com.github.yukon39.DebugBSL.data.core;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client.Internal.Tests
{
    class DebuggerClientExecutorTests
    {
        [Test]
        public async Task TestExecutorEmptyContent()
        {
            // given
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            responseMessage.Content = new StringContent("");

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(responseMessage);

            var executor = messageHandler.CreateExecutor();
            var parameters = new RequestParameters("test");

            // when
            var response = await executor.ExecuteAsync<RDBGEmptyResponse>(new RDBGTestRequest(), parameters);

            // then
            Assert.IsInstanceOf<RDBGEmptyResponse>(response);
        }

        [Test]
        public async Task TestExecutorWithContent()
        {
            // given
            var sampleResponse = new MiscRDbgGetAPIVerResponse()
            {
                Version = "1.2.3.4"
            };

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, sampleResponse);

            var executor = messageHandler.CreateExecutor();
            var parameters = new RequestParameters("test");

            // when
            var response = await executor.ExecuteAsync<MiscRDbgGetAPIVerResponse>(new RDBGTestRequest(), parameters);

            // then
            Assert.IsInstanceOf<MiscRDbgGetAPIVerResponse>(response);
        }

        [Test]
        public void TestExecutorWithVRSException()
        {
            // given
            var badRequest = HttpStatusCode.BadRequest;
            var vrsException = new VRSException()
            {
                Reason = (int)badRequest,
                Description = "Test"
            };

            var content = DebuggerXmlSerializer.Serialize(vrsException);
            var responseMessage = new HttpResponseMessage(badRequest);
            responseMessage.Content = new StringContent(content);

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(responseMessage);

            var executor = messageHandler.CreateExecutor();
            var parameters = new RequestParameters("test");

            // when
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await executor.ExecuteAsync<MiscRDbgGetAPIVerResponse>(new RDBGTestRequest(), parameters));

            // then
            Assert.AreEqual("Test", ex.Message);
        }
    }
}
