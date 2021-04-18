using com.github.yukon39.CoverageBSL.data;
using com.github.yukon39.CoverageBSL.data.core;
using com.github.yukon39.CoverageBSL.debugger;
using com.github.yukon39.CoverageBSL.debugger.debugRDBGRequestResponse;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL.httpDebug
{
    class HTTPDebugClient : IDebuggerClient
    {
        private static readonly HttpClient Client = new();

        private Uri DebugServerURL;

        private HTTPDebugClient(Uri debugServerURL)
        {
            DebugServerURL = debugServerURL;
        }

        public void Test()
        {
            var RequestParameters = new RequestParameters
            {
                Resource = "rdbgTest",
                Command = "test"
            };

            var Request = new RDBGTestRequest();

            Execute<RDBGEmptyResponse>(Request, RequestParameters);
        }

        public string ApiVersion()
        {
            var RequestParameters = new RequestParameters
            {
                Command = "getRDbgAPIVer"
            };

            var Request = new MiscRDbgGetAPIVerRequest();

            var Response = Execute<MiscRDbgGetAPIVerResponse>(Request, RequestParameters);
            var Version = Response.Version;

            return Version;
        }

        public IDebuggerSession CreateSession(string infobaseAlias)
        {
            return new HTTPDebugSession(this, infobaseAlias);
        }

        public T Execute<T>(IRDBGRequest request, RequestParameters requestParameters)
        {
            var CommandURL = string.Format("{0}e1crdbg/{1}", DebugServerURL, requestParameters.ToString());

            var RequestContent = HTTPDebugSerializer.Serialize(request);

            var ResponseContent = HttpResponseContent(CommandURL, RequestContent).ConfigureAwait(false).GetAwaiter().GetResult();

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
            HttpRequest.Headers.Accept.Add(new("application/xml"));
            HttpRequest.Headers.AcceptEncoding.Add(new("gzip"));

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
