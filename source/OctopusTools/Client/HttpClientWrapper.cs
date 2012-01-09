using System;
using System.IO;
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
        ICredentials credentials;

        public HttpClientWrapper(ILog log)
        {
            this.log = log;
        }

        public void SetAuthentication(ICredentials newCredentials)
        {
            credentials = newCredentials;
        }

        public Task<TResult> Get<TResult>(Uri uri)
        {
            return new Task<TResult>(
                delegate
                {
                    var request = CreateWebRequest("GET", uri);

                    var response = request.GetResponse();
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        return JsonConvert.DeserializeObject<TResult>(content);
                    }
                });
        }

        public Task<TResource> Post<TResource>(Uri uri, TResource resource)
        {
            return new Task<TResource>(
                delegate
                {
                    var postData = JsonConvert.SerializeObject(resource, Formatting.Indented);

                    var request = CreateWebRequest("POST", uri);
                    request.ContentType = "application/json";
                    AppendBody(request, postData);

                    var response = ReadResponse(request);
                    var location = response.Headers.Get("Location");
                    return Get<TResource>(new Uri(uri, location)).Execute();
                });
        }

        public Task<TResource> Put<TResource>(Uri uri, TResource resource)
        {
            return new Task<TResource>(
                delegate
                {
                    var postData = JsonConvert.SerializeObject(resource, Formatting.Indented);

                    var request = CreateWebRequest("POST", uri);
                    request.ContentType = "application/json";
                    request.Headers["X-HTTP-Method-Override"] = "PUT";
                    AppendBody(request, postData);

                    ReadResponse(request);

                    return Get<TResource>(uri).Execute();
                });
        }

        WebRequest CreateWebRequest(string method, Uri uri)
        {
            log.Debug(method + " " + uri);

            var request = WebRequest.Create(uri);
            request.Credentials = credentials;
            request.Method = method;

            return request;
        }

        void AppendBody(WebRequest request, string body)
        {
            if (!string.IsNullOrWhiteSpace(body))
            {
                using (var requestStream = new StreamWriter(request.GetRequestStream()))
                {
                    requestStream.WriteLine(body);
                }
            }
        }

        static WebResponse ReadResponse(WebRequest request)
        {
            try
            {
                return request.GetResponse();
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var reader = new StreamReader(wex.Response.GetResponseStream()))
                    {
                        var details = reader.ReadToEnd();

                        if (!string.IsNullOrWhiteSpace(details))
                        {
                            throw new Exception(wex.Message + " " + details);
                        }
                    }
                }

                throw;
            }
        }
    }
}