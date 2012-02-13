using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Client
{
    public class OctopusSession : IOctopusSession
    {
        readonly Lazy<RootDocument> rootDocument;
        readonly Uri serverBaseUri;
        readonly ICredentials credentials;
        readonly ILog log;

        public OctopusSession(Uri serverBaseUri, ICredentials credentials, ILog log)
        {
            this.serverBaseUri = serverBaseUri;
            this.credentials = credentials;
            this.log = log;

            rootDocument = new Lazy<RootDocument>(EstablishSession);
        }

        public RootDocument RootDocument
        {
            get { return rootDocument.Value; }
        }

        public void Initialize()
        {
            rootDocument.LoadValue();
        }

        public IList<TResource> List<TResource>(string path)
        {
            return Get<IList<TResource>>(path);
        }

        public TResource Get<TResource>(string path)
        {
            var uri = QualifyUri(path);

            var request = CreateWebRequest("GET", uri);

            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<TResource>(content);
                }
            }
        }

        public TResource Create<TResource>(string path, TResource resource)
        {
            var uri = QualifyUri(path);

            var postData = JsonConvert.SerializeObject(resource, Formatting.Indented);

            var request = CreateWebRequest("POST", uri);
            request.ContentType = "application/json";
            AppendBody(request, postData);

            using (var response = ReadResponse(request))
            {
                var location = response.Headers.Get("Location");
                return Get<TResource>(location);
            }
        }

        public TResource Update<TResource>(string path, TResource resource)
        {
            var uri = QualifyUri(path);

            var postData = JsonConvert.SerializeObject(resource, Formatting.Indented);

            var request = CreateWebRequest("POST", uri);
            request.ContentType = "application/json";
            request.Headers["X-HTTP-Method-Override"] = "PUT";
            AppendBody(request, postData);

            using (ReadResponse(request)) { }

            return Get<TResource>(uri.AbsolutePath);
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

        RootDocument EstablishSession()
        {
            log.Debug("Handshaking with Octopus server: " + serverBaseUri);
            var server = Get<RootDocument>("/api");
            log.Debug("Handshake successful. Octopus version: " + server.Version);
            return server;
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

        public void Dispose()
        {
        }
    }
}
