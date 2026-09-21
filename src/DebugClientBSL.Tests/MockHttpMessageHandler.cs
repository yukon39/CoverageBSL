using com.github.yukon39.DebugBSL.Client.Internal;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client.Tests
{
    class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<Task<HttpResponseMessage>> responses = new Queue<Task<HttpResponseMessage>>();

        private readonly Queue<HttpRequestMessage> requests = new Queue<HttpRequestMessage>();

        public void Enqueue(HttpResponseMessage responseMessage)
            => responses.Enqueue(Task.FromResult(responseMessage));

        public void EnqueueCanceled()
            => responses.Enqueue(Task.FromCanceled<HttpResponseMessage>(new CancellationToken(true)));

        public void Enqueue(HttpStatusCode statusCode, IRDBGResponse response)
        {
            var content = DebuggerXmlSerializer.Serialize(response);
            var responseMessage = new HttpResponseMessage(statusCode);
            responseMessage.Content = new StringContent(content);
            Enqueue(responseMessage);
        }

        public int RequestCount
        {
            get
            {
                lock (requests)
                {
                    return requests.Count;
                }
            }
        }

        public HttpRequestMessage Dequeue()
        {
            lock (requests)
            {
                return requests.Dequeue();
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            lock (requests)
            {
                requests.Enqueue(request);
            }
            var response = responses.Dequeue();
            return response;
        }


        public DebuggerClientExecutor CreateExecutor()
        {
            var httpClient = new HttpClient(this);
            var rootUrl = new Uri("http://localhost");
            var executor = DebuggerClientExecutor.Create(rootUrl, httpClient);

            return executor;
        }
    }
}
