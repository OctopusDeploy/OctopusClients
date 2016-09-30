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
        readonly object requestResource;

        public OctopusRequest(string method, Uri uri, object requestResource = null)
        {
            this.method = method;
            this.uri = uri;
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


        public object RequestResource
        {
            get { return requestResource; }
        }
    }
}