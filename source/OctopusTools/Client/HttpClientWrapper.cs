using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using log4net;

namespace OctopusTools.Client
{
    public class HttpClientWrapper : IHttpClient
    {
        readonly ILog log;
        readonly HttpClient client;

        public HttpClientWrapper(ILog log)
        {
            this.log = log;
            client = new HttpClient();
        }

        public void SetAuthentication(ICredentials credentials, string apiKey)
        {
            
        }

        public Task<TResult> Get<TResult>(Uri uri)
        {
            return new Task<TResult>(
                delegate
                {
                    var response = client.Get(uri);
                    response.EnsureSuccessStatusCode();
                    var content = response.Content.ReadAsString();
                    return JsonConvert.DeserializeObject<TResult>(content);
                });
        }

        public Task<TResource> Post<TResource>(Uri uri, TResource resource)
        {
            return new Task<TResource>(
            delegate
            {
                var request = JsonConvert.SerializeObject(resource, Formatting.Indented);

                var response = client.Post(uri, new StringContent(request, Encoding.UTF8, "application/json"));
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var content = response.Content.ReadAsString();

                    throw new Exception("The server rejected the POST request: " + content);
                }

                response.EnsureSuccessStatusCode();
                var location = response.Headers.Location;
                return Get<TResource>(new Uri(uri, location)).Execute();
            });
        }

        public Task<TResource> Put<TResource>(Uri uri, TResource resource)
        {
            return new Task<TResource>(
            delegate
            {
                var request = JsonConvert.SerializeObject(resource, Formatting.Indented);

                var response = client.Put(uri, new StringContent(request, Encoding.UTF8, "application/json"));
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var content = response.Content.ReadAsString();

                    throw new Exception("The server rejected the PUT request: " + content);
                }

                response.EnsureSuccessStatusCode();
                return Get<TResource>(uri).Execute();
            });
        }
    }
}