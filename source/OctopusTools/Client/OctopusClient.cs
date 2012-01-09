using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Client
{
    public class OctopusClient : IOctopusClient
    {
        readonly Uri serverBaseUri;
        readonly ICredentials credentials;
        readonly ILog log;

        public OctopusClient(Uri serverBaseUri, ICredentials credentials, ILog log)
        {
            this.serverBaseUri = serverBaseUri;
            this.credentials = credentials;
            this.log = log;
        }

        public Task<OctopusInstance> Handshake()
        {
            return new Task<OctopusInstance>(() =>
            {
                log.Debug("Handshaking with Octopus server: " + serverBaseUri);
                var task = Get<OctopusInstance>("/api");
                task.RunSynchronously();
                log.Debug("Handshake successful. Octopus version: " + task.Result.Version);
                return task.Result;
            });
        }

        public Task<IList<TResource>> List<TResource>(string path)
        {
            return Get<IList<TResource>>(path);
        }

        public Task<TResource> Get<TResource>(string path)
        {
            var uri = QualifyUri(path);

            return new Task<TResource>(
                delegate
                {
                    var request = CreateWebRequest("GET", uri);

                    var response = request.GetResponse();
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        return JsonConvert.DeserializeObject<TResource>(content);
                    }
                });
        }

        public Task<TResource> Create<TResource>(string path, TResource resource)
        {
            var uri = QualifyUri(path);

            return new Task<TResource>(
                delegate
                {
                    var postData = JsonConvert.SerializeObject(resource, Formatting.Indented);

                    var request = CreateWebRequest("POST", uri);
                    request.ContentType = "application/json";
                    AppendBody(request, postData);

                    var response = ReadResponse(request);
                    var location = response.Headers.Get("Location");
                    return Get<TResource>(location).Execute();
                });
        }

        public Task<TResource> Update<TResource>(string path, TResource resource)
        {
            var uri = QualifyUri(path);

            return new Task<TResource>(
                delegate
                {
                    var postData = JsonConvert.SerializeObject(resource, Formatting.Indented);

                    var request = CreateWebRequest("POST", uri);
                    request.ContentType = "application/json";
                    request.Headers["X-HTTP-Method-Override"] = "PUT";
                    AppendBody(request, postData);

                    ReadResponse(request);

                    return Get<TResource>(uri.AbsolutePath).Execute();
                });
        }

        Uri QualifyUri(string path)
        {
            return new Uri(serverBaseUri, path);
        }

        WebRequest CreateWebRequest(string method, Uri uri)
        {
            var request = WebRequest.Create(uri);
            request.Credentials = credentials;
            request.Method = method;
            return request;
        }

        static void AppendBody(WebRequest request, string body)
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
