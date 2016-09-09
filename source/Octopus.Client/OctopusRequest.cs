using System;
using System.Collections.Generic;

namespace Octopus.Client
{
    /// <summary>
    /// Describes a request made to the Octopus server by the client.
    /// </summary>
    public class OctopusRequest
    {
        readonly string method;
        readonly Uri uri;
        readonly IDictionary<string, string> requestHeaders;
        readonly object requestResource;

        public OctopusRequest(string method, Uri uri, IDictionary<string, string> requestHeaders = null, object requestResource = null)
        {
            this.method = method;
            this.uri = uri;
            this.requestHeaders = new Dictionary<string, string>(requestHeaders ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
            this.requestResource = requestResource;
        }

        public string Method
        {
            get { return method; }
        }

        public Uri Uri
        {
            get { return uri; }
        }

        public IDictionary<string, string> RequestHeaders
        {
            get { return requestHeaders; }
        }

        public object RequestResource
        {
            get { return requestResource; }
        }
    }
}