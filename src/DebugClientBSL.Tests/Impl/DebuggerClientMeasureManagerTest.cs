using com.github.yukon39.DebugBSL.Client.Data;
using com.github.yukon39.DebugBSL.Client.Impl;
using com.github.yukon39.DebugBSL.Client.Tests;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugClientBSL.Impl.Tests
{
    class DebuggerClientMeasureManagerTest
    {
        [Test]
        public async Task TestStartMeasureModeAsync()
        {
            // given
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var manager = Create(messageHandler);

            // when
            var measureId = await manager.StartMeasureModeAsync();

            // then
            Assert.That(measureId, Is.InstanceOf<Guid>());
            Assert.That(measureId, Is.Not.EqualTo(Guid.Empty));

            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo("http://localhost/e1crdbg/rdbg?cmd=setMeasureMode"));
        }

        [Test]
        public async Task TestStartMeasureModeWitGuidAsync()
        {
            // given
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var manager = Create(messageHandler);
            var measureId = Guid.NewGuid();

            // when
            await manager.StartMeasureModeAsync(measureId);

            // then
            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo("http://localhost/e1crdbg/rdbg?cmd=setMeasureMode"));
        }

        private static DebuggerClientMeasureManager Create(MockHttpMessageHandler messageHandler)
        {

            var executor = messageHandler.CreateExecutor();
            var context = SessionContext.NewInstance("testAlias");

            return new DebuggerClientMeasureManager(executor, context);
        }
    }
}
