using System;
using System.Net;

namespace Octopus.Client
{
    /// <summary>
    /// Describes a response from the Octopus Server.
    /// </summary>
    /// <typeparam name="TResponseResource">The resource type associated with the response.</typeparam>
    public class OctopusResponse<TResponseResource> : OctopusResponse
    {
        public OctopusResponse(OctopusRequest request, HttpStatusCode statusCode, string location, TResponseResource responseResource) : base(request, statusCode, location, responseResource)
        {
        }

        public new TResponseResource ResponseResource
        {
            get { return (TResponseResource)base.ResponseResource; }
        }
    }

    /// <summary>
    /// Describes a response from the Octopus Server.
    /// </summary>
    public class OctopusResponse
    {
        readonly OctopusRequest request;
        readonly HttpStatusCode statusCode;
        readonly string location;
        readonly object responseResource;

        public OctopusResponse(OctopusRequest request, HttpStatusCode statusCode, string location, object responseResource)
        {
            this.request = request;
            this.statusCode = statusCode;
            this.location = location;
            this.responseResource = responseResource;
        }

        public OctopusRequest Request
        {
            get { return request; }
        }

        public HttpStatusCode StatusCode
        {
            get { return statusCode; }
        }

        public string Location
        {
            get { return location; }
        }

        public object ResponseResource
        {
            get { return responseResource; }
        }
    }
}