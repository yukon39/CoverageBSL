using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.github.yukon39.DebugClientBSL
{
    class RequestParameters
    {
        private readonly Dictionary<string, string> Parameters = new Dictionary<string, string>();

        public RequestParameters(Uri rootUrl, string command) : this(rootUrl, command, "rdbg") { }

        public RequestParameters(Uri rootUrl, string command, string resource)
        {
            RootUrl = rootUrl;
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

        public string Resource { get; set; }

        public Uri RootUrl { get; }

        public override string ToString()
        {
            var URLParams = Parameters.Select(x => string.Format("{0}={1}", x.Key, x.Value));

            var Result = new StringBuilder();
            Result.Append(Resource);
            Result.Append('?');
            Result.Append(string.Join("&", URLParams));

            return Result.ToString();
        }
    }
}
