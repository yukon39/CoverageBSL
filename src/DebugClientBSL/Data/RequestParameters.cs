using System;
using System.Collections.Generic;
using System.Linq;

namespace com.github.yukon39.DebugBSL.Client.Data
{
    public class RequestParameters
    {
        private readonly string Resource;
        private readonly Dictionary<string, string> Parameters = new Dictionary<string, string>();

        public RequestParameters(string command) : this(command, "rdbg") { }

        public RequestParameters(string command, string resource)
        {
            Resource = resource;
            Parameters.Add("cmd", command);
        }

        public Guid DebugID
        {
            set
            {
                Parameters.Add("dbgui", value.ToString());
            }
        }

        public Uri RequestUrl(Uri rootUrl)
        {
            var query = Parameters.Select(x => string.Format("{0}={1}", x.Key, x.Value));

            var uriBuilder = new UriBuilder(rootUrl);
            uriBuilder.Path = string.Format("e1crdbg/{0}", Resource);
            uriBuilder.Query = string.Join("&", query);

            return uriBuilder.Uri;
        }
    }
}
