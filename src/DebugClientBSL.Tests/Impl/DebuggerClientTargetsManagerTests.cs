using com.github.yukon39.DebugBSL.Client.Data;
using com.github.yukon39.DebugBSL.Client.Tests;
using com.github.yukon39.DebugBSL.debugger.debugAutoAttach;
using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client.Impl.Tests
{
    class DebuggerClientTargetsManagerTests
    {
        [Test]
        public async Task TestAttachDebugTargetAsync()
        {
            // given
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var manager = Create(messageHandler);
            var debugTargetId = new DebugTargetIdLight()
            {
                ID = Guid.NewGuid()
            };

            // when
            await manager.AttachDebugTargetAsync(debugTargetId);

            // then
            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo("http://localhost/e1crdbg/rdbg?cmd=attachDetachDbgTargets"));
        }

        [Test]
        public async Task TestDetachDebugTargetAsync()
        {
            // given
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var manager = Create(messageHandler);
            var debugTargetId = new DebugTargetIdLight()
            {
                ID = Guid.NewGuid()
            };

            // when
            await manager.DetachDebugTargetAsync(debugTargetId);

            // then
            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo("http://localhost/e1crdbg/rdbg?cmd=attachDetachDbgTargets"));
        }

        [Test]
        public async Task TestAttachedTargetsStatesAsync()
        {
            // given
            var response = new RDBGGetDbgAllTargetStatesResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var manager = Create(messageHandler);
            var areaName = "";

            // when
            var result = await manager.AttachedTargetsStatesAsync(areaName);

            // then
            Assert.That(result, Is.InstanceOf<List<DbgTargetStateInfo>>());

            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo("http://localhost/e1crdbg/rdbg?cmd=getDbgAllTargetStates"));
        }

        [Test]
        public async Task TestInitSettingsAsync()
        {
            // given
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var manager = Create(messageHandler);

            var data = new HTTPServerInitialDebugSettingsData();

            // when
            await manager.InitSettingsAsync(data);

            // then
            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo("http://localhost/e1crdbg/rdbg?cmd=initSettings"));
        }

        [Test]
        public async Task TestSetAutoAttachSettingsAsync()
        {
            // given
            var response = new RDBGEmptyResponse();

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.Enqueue(HttpStatusCode.OK, response);

            var manager = Create(messageHandler);
            var autoAttachSettings = new DebugAutoAttachSettings();

            // when
            await manager.SetAutoAttachSettingsAsync(autoAttachSettings);

            // then
            var request = messageHandler.Dequeue();
            Assert.That(request.RequestUri.ToString(), Is.EqualTo("http://localhost/e1crdbg/rdbg?cmd=setAutoAttachSettings"));
        }

        private static DebuggerClientTargetsManager Create(MockHttpMessageHandler messageHandler)
        {

            var executor = messageHandler.CreateExecutor();
            var context = SessionContext.NewInstance("testAlias");

            return new DebuggerClientTargetsManager(executor, context);
        }
    }
}
