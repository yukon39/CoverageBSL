using com.github.yukon39.DebugBSL;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugClientBSL.Tests
{
    class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> responses = new Queue<HttpResponseMessage>();

        private readonly Queue<HttpRequestMessage> requests = new Queue<HttpRequestMessage>();

        public void Enqueue(HttpResponseMessage responseMessage)
            => responses.Enqueue(responseMessage);

        public void Enqueue(HttpStatusCode statusCode, IRDBGResponse response)
        {
            var content = DebuggerXmlSerializer.Serialize(response);
            var responseMessage = new HttpResponseMessage(statusCode);
            responseMessage.Content = new StringContent(content);
            Enqueue(responseMessage);
        }

        public HttpRequestMessage Dequeue() =>
            requests.Dequeue();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            requests.Enqueue(request);
            var response = responses.Dequeue();
            return Task.FromResult(response);
        } 
             

        public HttpClientExecutor CreateExecutor()
        {
            var httpClient = new HttpClient(this);
            var rootUrl = new Uri("http://localhost");
            var executor = HttpClientExecutor.Create(rootUrl, httpClient);

            return executor;
        }
    }
}
