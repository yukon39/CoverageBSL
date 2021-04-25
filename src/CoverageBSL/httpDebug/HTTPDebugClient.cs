using com.github.yukon39.DebugBSL;
using com.github.yukon39.DebugBSL.data;
using com.github.yukon39.DebugBSL.data.core;
using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL.httpDebug
{
    class HTTPDebugClient : IDebuggerClient
    {
        private static readonly HttpClient Client = new HttpClient();

        private readonly Uri DebugServerURL;

        private HTTPDebugClient(Uri debugServerURL)
        {
            DebugServerURL = debugServerURL;
        }

        public async Task TestAsync()
        {
            var requestParameters = new RequestParameters
            {
                Resource = "rdbgTest",
                Command = "test"
            };

            var request = new RDBGTestRequest();

            await ExecuteAsync<RDBGEmptyResponse>(request, requestParameters);
        }

        public async Task<string> ApiVersionAsync()
        {
            var requestParameters = new RequestParameters
            {
                Command = "getRDbgAPIVer"
            };

            var request = new MiscRDbgGetAPIVerRequest();

            var response = await ExecuteAsync<MiscRDbgGetAPIVerResponse>(request, requestParameters);
            var version = response.Version;

            return version;
        }

        public IDebuggerClientSession CreateSession(string infobaseAlias)
        {
            return new HTTPDebugSession(this, infobaseAlias);
        }

        public async Task<T> ExecuteAsync<T>(IRDBGRequest request, IDebuggerClientRequestParameters parameters) where T : IRDBGResponse
        {
            var CommandURL = string.Format("{0}e1crdbg/{1}", DebugServerURL, parameters.ToString());

            var RequestContent = HTTPDebugSerializer.Serialize(request);

            var ResponseContent = await HttpResponseContent(CommandURL, RequestContent);

            if (string.IsNullOrEmpty(ResponseContent))
            {
                return Activator.CreateInstance<T>();
            }
            else
            {
                return HTTPDebugSerializer.Deserialize<T>(ResponseContent);
            }
        }

        private static async Task<string> HttpResponseContent(string url, string requestContent)
        {

            var Content = new StringContent(requestContent, Encoding.UTF8, "application/xml");
            //Content.Headers.ContentType = new("application/xml; charset=utf-8");

            var HttpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            HttpRequest.Content = Content;
            HttpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            HttpRequest.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var HttpResponse = await Client.SendAsync(HttpRequest);

            var ResponseContentString = await HttpResponse.Content.ReadAsStringAsync();
            if (!HttpResponse.IsSuccessStatusCode)
            {
                var exception = HTTPDebugSerializer.Deserialize<VRSException>(ResponseContentString);
                var description = ErrorProcessingManager.BriefErrorDescription(exception);
                throw new Exception(description);
            }

            return ResponseContentString;
        }

        public static IDebuggerClient Build(Uri debugServerURL) => new HTTPDebugClient(debugServerURL);
    }
}
