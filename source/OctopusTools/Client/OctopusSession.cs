using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Client
{
    public class OctopusSession : IOctopusSession
    {
        readonly Lazy<RootDocument> rootDocument;
        readonly Uri serverBaseUri;
        readonly ICredentials credentials;
        readonly string apiKey;
        readonly ILog log;
        readonly JsonSerializerSettings serializerSettings;

        public OctopusSession(Uri serverBaseUri, ICredentials credentials, string apiKey, ILog log)
        {
            this.serverBaseUri = serverBaseUri;
            this.credentials = credentials;
            this.apiKey = apiKey;
            this.log = log;

            serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());

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

        public string QualifyWebLink(string path)
        {
            return Regex.Replace(serverBaseUri.ToString(), "/api$", path);
        }

        public IList<TResource> List<TResource>(string path)
        {
            return Get<IList<TResource>>(path);
        }

        public IList<TResource> List<TResource>(string path, QueryString queryString)
        {
            return Get<IList<TResource>>(path, queryString);
        }

        public TResource Get<TResource>(string path)
        {
            return Get<TResource>(path, null);
        }

        public TResource Get<TResource>(string path, QueryString queryString)
        {
            var uri = QualifyUri(path, queryString);

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
            var postData = JsonConvert.SerializeObject(resource, Formatting.Indented, serializerSettings);

            var request = CreateWebRequest("POST", uri);
            request.ContentType = "application/json";
            AppendBody(request, postData);

            using (var response = ReadResponse(request))
            {
                var location = response.Headers.Get("Location");
                if (location == null)
                {
                    throw new Exception("Unexpected response: " + new StreamReader(response.GetResponseStream()).ReadToEnd());
                }

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

        public void Delete<TResource>(string path)
        {
            var uri = QualifyUri(path);

            var request = CreateWebRequest("POST", uri);
            request.ContentLength = 0;
            request.Headers["X-HTTP-Method-Override"] = "DELETE";

            using (ReadResponse(request)) { }
        }

        Uri QualifyUri(string path, QueryString queryString = null)
        {
            if (queryString != null && queryString.Count > 0)
            {
                var isFirstParam = !path.Contains("?");
                foreach (var pair in queryString)
                {
                    path += (isFirstParam ? "?" : "&") + pair.Key + "=" + HttpUtility.UrlEncode((pair.Value ?? string.Empty).ToString());
                    isFirstParam = false;
                }
            }

            return serverBaseUri.EnsureEndsWith(path);
        }

        WebRequest CreateWebRequest(string method, Uri uri)
        {
            var request = WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Credentials = credentials;
            request.Method = method;
            request.Headers["X-Octopus-ApiKey"] = apiKey;
            return request;
        }

        RootDocument EstablishSession()
        {
            log.Debug("Handshaking with Octopus server: " + serverBaseUri);
            var server = Get<RootDocument>("/api");
            log.Debug("Handshake successful. Octopus version: " + server.Version + "; API version: " + server.ApiVersion );

            if (string.IsNullOrWhiteSpace(server.ApiVersion))
                throw new CommandException("This Octopus server uses a newer API specification than this tool can handle. Please check for updates to the Octo tool.");
            
            var min = SemanticVersion.Parse(ApiConstants.SupportedApiSchemaVersionMin);
            var max = SemanticVersion.Parse(ApiConstants.SupportedApiSchemaVersionMax);
            var current = SemanticVersion.Parse(server.ApiVersion);

            if (current < min || current > max)
                throw new CommandException(string.Format("This Octopus server uses a newer API specification ({0}) than this tool can handle ({1} to {2}). Please check for updates to the Octo tool.", server.ApiVersion, ApiConstants.SupportedApiSchemaVersionMin, ApiConstants.SupportedApiSchemaVersionMax));

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

                        var message = wex.Message + " " + details;
                        var header = wex.Response.Headers["X-Error"];
                        if (header != null)
                        {
                            message = header + " " + details;
                        }

                        if (!string.IsNullOrWhiteSpace(message))
                        {
                            throw new Exception(message);
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
