using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class OctopusAsyncTestClient : OctopusAsyncClient
    {
        readonly JsonSerializerSettings defaultJsonSerializerSettings = JsonSerialization.GetDefaultSerializerSettings();

        public OctopusAsyncTestClient() : base(GetServerEndpoint(), new OctopusClientOptions(), addCertificateCallback: false)
        {
            GetStatusCode = () => HttpStatusCode.OK;
            GetLocationHeader = () => "";
        }

        public Func<HttpStatusCode> GetStatusCode { get; set; }
        public Func<string> GetLocationHeader { get; set; }

        private static OctopusServerEndpoint GetServerEndpoint()
        {
            return new OctopusServerEndpoint("http://locahost");
        }

        protected override async Task<OctopusResponse<TResponseResource>> DispatchRequest<TResponseResource>(OctopusRequest request, bool readResponse)
        {
            var escapedUri = request.Uri.PathAndQuery
                .Replace("/api/", "")
                .Replace("/", ".")
                .Replace("?", ".")
                .Replace("&", ".")
                .Replace("-", "_");
            var resourceName = $"Octopus.Client.Tests.CannedResponses.{escapedUri}.{request.Method}.json";
            var assembly = typeof(OctopusAsyncTestClient).GetTypeInfo().Assembly;

            using (var responseStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (responseStream == null)
                {
                    var validResources = assembly.GetManifestResourceNames();
                    throw new Exception($"Didn't find a canned response '{resourceName}'.{Environment.NewLine}Valid resources names are: {Environment.NewLine}{string.Join(Environment.NewLine, validResources)}");
                }

                using (var reader = new StreamReader(responseStream))
                {
                    var content = reader.ReadToEnd();
                    var resource = JsonConvert.DeserializeObject<TResponseResource>(content, defaultJsonSerializerSettings);
                    return await Task.Run(() => new OctopusResponse<TResponseResource>(request, GetStatusCode(), GetLocationHeader(), resource));
                }
            }
        }

        protected override async Task<RootResource> EstablishSession()
        {
            return await Task.Run(() => new RootResource());
        }
    }
}
