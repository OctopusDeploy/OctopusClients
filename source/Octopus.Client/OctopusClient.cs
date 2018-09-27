#if SYNC_CLIENT
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Serialization;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensions;

namespace Octopus.Client
{
    /// <summary>
    /// The Octopus Deploy RESTful HTTP API client.
    /// </summary>
    public class OctopusClient : IHttpOctopusClient
    {
        readonly OctopusServerEndpoint serverEndpoint;
        CookieContainer cookieContainer = new CookieContainer();
        readonly Uri cookieOriginUri;
        readonly JsonSerializerSettings defaultJsonSerializerSettings = JsonSerialization.GetDefaultSerializerSettings();
        readonly SemanticVersion clientVersion;
        private Lazy<IOctopusRepository> lazyRepository;
        bool signedIn = false;
        public bool IsAuthenticated => (signedIn || !string.IsNullOrEmpty(this.serverEndpoint.ApiKey));

        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusClient" /> class.
        /// </summary>
        /// <param name="serverEndpoint">The server endpoint.</param>
        public OctopusClient(OctopusServerEndpoint serverEndpoint)
        {
            this.serverEndpoint = serverEndpoint;
            cookieOriginUri = BuildCookieUri(serverEndpoint);
            clientVersion = GetType().GetSemanticVersion();
            lazyRepository = new Lazy<IOctopusRepository>(LoadRepository, true);
        }

        public IOctopusRepository Repository => lazyRepository.Value;
        /// <summary>
        /// Indicates whether a secure (SSL) connection is being used to communicate with the server.
        /// </summary>

        public bool IsUsingSecureConnection => serverEndpoint.IsUsingSecureConnection;

        [Obsolete("This property is deprecated, please the one from Repository instead")]
        public RootResource RootDocument => Repository.RootDocument;

        [Obsolete("This method is deprecated, please the one from Repository instead")]
        public RootResource RefreshRootDocument()
        {
            return Repository.RefreshRootDocument();
        }

        public void SignIn(LoginCommand loginCommand)
        {
            if (loginCommand.State == null)
            {
                loginCommand.State = new LoginState { UsingSecureConnection = IsUsingSecureConnection };
            }
            Post(Repository.RootDocument.Links["SignIn"], loginCommand);
            signedIn = true;
            lazyRepository = new Lazy<IOctopusRepository>(LoadRepository, true);
        }

        public void SignOut()
        {
            Post(Repository.RootDocument.Links["SignOut"]);
            signedIn = false;
        }

        /// <summary>
        /// Occurs when a request is about to be sent.
        /// </summary>
        public event Action<WebRequest> BeforeSendingHttpRequest;

        /// <summary>
        /// Occurs when a response has been received.
        /// </summary>
        public event Action<WebResponse> AfterReceivingHttpResponse;

        /// <summary>
        /// Occurs when a request is about to be sent.
        /// </summary>
        public event Action<OctopusRequest> SendingOctopusRequest;

        /// <summary>
        /// Occurs when a response is received from the Octopus server.
        /// </summary>
        public event Action<OctopusResponse> ReceivedOctopusResponse;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            // Force the Lazy instance to be loaded
            Repository.RootDocument.Link("Self");
        }

        private Uri BuildCookieUri(OctopusServerEndpoint octopusServerEndpoint)
        {
            // The CookieContainer is a bit funny - it sets the cookie without the port, but doesn't ignore the port when retreiving cookies
            // From what I can see it uses the Uri.Authority value - which contains the port number
            // We need to clear the port in order to successfully get cookies for the same origin
            var uriBuilder = new UriBuilder(octopusServerEndpoint.OctopusServer.Resolve("/")) { Port = 0 };

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Fetches a single resource from the server using the HTTP GET verb.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="path">The path from which to fetch the resource.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        /// <returns>
        /// The resource from the server.
        /// </returns>
        public TResource Get<TResource>(string path, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);

            return DispatchRequest<TResource>(new OctopusRequest("GET", uri), true).ResponseResource;
        }

        /// <summary>
        /// Fetches a collection of resources from the server using the HTTP GET verb. The collection itself will usually be
        /// limited in size (pagination) and links to the next page of data is available in the <see cref="Resource.Links" />
        /// property.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="path">The path from which to fetch the resources.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        /// <returns>
        /// The collection of resources from the server.
        /// </returns>
        public ResourceCollection<TResource> List<TResource>(string path, object pathParameters = null)
        {
            return Get<ResourceCollection<TResource>>(path, pathParameters);
        }

        /// <summary>
        /// Fetches a collection of resources from the server using the HTTP GET verb. All pages will be retrieved.
        /// property.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="path">The path from which to fetch the resources.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        /// <returns>
        /// The collection of resources from the server.
        /// </returns>
        public IReadOnlyList<TResource> ListAll<TResource>(string path, object pathParameters = null)
        {
            var resources = new List<TResource>();
            Paginate<TResource>(path, pathParameters, r =>
            {
                resources.AddRange(r.Items);
                return true;
            });
            return resources;
        }

        /// <summary>
        /// Fetches a collection of resources from the server one page at a time using the HTTP GET verb.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="path">The path from which to fetch the resources.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        /// <param name="getNextPage">
        /// A callback invoked for each page of data found. If the callback returns <c>true</c>, the next
        /// page will also be requested.
        /// </param>
        public void Paginate<TResource>(string path, object pathParameters, Func<ResourceCollection<TResource>, bool> getNextPage)
        {
            var page = List<TResource>(path, pathParameters);

            while (getNextPage(page) && page.Items.Count > 0 && page.HasLink("Page.Next"))
            {
                page = List<TResource>(page.Link("Page.Next"));
            }
        }

        /// <summary>
        /// Fetches a collection of resources from the server one page at a time using the HTTP GET verb.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="path">The path from which to fetch the resources.</param>
        /// <param name="getNextPage">
        /// A callback invoked for each page of data found. If the callback returns <c>true</c>, the next
        /// page will also be requested.
        /// </param>
        public void Paginate<TResource>(string path, Func<ResourceCollection<TResource>, bool> getNextPage)
        {
            Paginate(path, null, getNextPage);
        }

        /// <summary>
        /// Creates a resource at the given URI on the server using the POST verb, then performs a fresh GET request to fetch
        /// the created item.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="path">The path to the container resource.</param>
        /// <param name="resource">The resource to create.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        /// <returns>
        /// The latest copy of the resource from the server.
        /// </returns>
        public TResource Create<TResource>(string path, TResource resource, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);

            var response = DispatchRequest<TResource>(new OctopusRequest("POST", uri, requestResource: resource), true);
            return Get<TResource>(response.Location);
        }

        /// <summary>
        /// Sends a command to a resource at the given URI on the server using the POST verb.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="path">The path to the container resource.</param>
        /// <param name="resource">The resource to create.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        public void Post<TResource>(string path, TResource resource, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);

            DispatchRequest<TResource>(new OctopusRequest("POST", uri, requestResource: resource), false);
        }

        /// <summary>
        /// Sends a command to a resource at the given URI on the server using the POST verb.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="path">The path to the container resource.</param>
        /// <param name="resource">The resource to post.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        public TResponse Post<TResource, TResponse>(string path, TResource resource, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);

            return DispatchRequest<TResponse>(new OctopusRequest("POST", uri, requestResource: resource), true).ResponseResource;
        }

        /// <summary>
        /// Sends a command to a resource at the given URI on the server using the POST verb.
        /// </summary>
        /// <param name="path">The path to the container resource.</param>
        public void Post(string path)
        {
            var uri = QualifyUri(path);

            DispatchRequest<string>(new OctopusRequest("POST", uri), false);
        }

        /// <summary>
        /// Sends a command to a resource at the given URI on the server using the PUT verb.
        /// </summary>
        /// <exception cref="OctopusSecurityException">
        /// HTTP 401 or 403: Thrown when the current user's API key was not valid, their
        /// account is disabled, or they don't have permission to perform the specified action.
        /// </exception>
        /// <exception cref="OctopusServerException">
        /// If any other error is successfully returned from the server (e.g., a 500
        /// server error).
        /// </exception>
        /// <exception cref="OctopusValidationException">HTTP 400: If there was a problem with the request provided by the user.</exception>
        /// <exception cref="OctopusResourceNotFoundException">HTTP 404: If the specified resource does not exist on the server.</exception>
        /// <param name="path">The path to the container resource.</param>
        /// <param name="resource">The resource to create.</param>
        public void Put<TResource>(string path, TResource resource)
        {
            var uri = QualifyUri(path);

            DispatchRequest<TResource>(new OctopusRequest("PUT", uri, requestResource: resource), false);
        }

        /// <summary>
        /// Sends a command to a resource at the given URI on the server using the PUT verb.
        /// </summary>
        /// <exception cref="OctopusSecurityException">
        /// HTTP 401 or 403: Thrown when the current user's API key was not valid, their
        /// account is disabled, or they don't have permission to perform the specified action.
        /// </exception>
        /// <exception cref="OctopusServerException">
        /// If any other error is successfully returned from the server (e.g., a 500
        /// server error).
        /// </exception>
        /// <exception cref="OctopusValidationException">HTTP 400: If there was a problem with the request provided by the user.</exception>
        /// <exception cref="OctopusResourceNotFoundException">HTTP 404: If the specified resource does not exist on the server.</exception>
        /// <param name="path">The path to the container resource.</param>
        public void Put(string path)
        {
            var uri = QualifyUri(path);

            DispatchRequest<string>(new OctopusRequest("PUT", uri), false);
        }

        /// <summary>
        /// Deletes the resource at the given URI from the server using a the DELETE verb.
        /// </summary>
        /// <param name="path">The path to the resource to delete.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        public void Delete(string path, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);

            DispatchRequest<string>(new OctopusRequest("DELETE", uri), true);
        }

        /// <summary>
        /// Updates the resource at the given URI on the server using the PUT verb, then performs a fresh GET request to reload
        /// the data.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="path">The path to the resource to update.</param>
        /// <param name="resource">The resource to update.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        /// <returns>
        /// The latest copy of the resource from the server.
        /// </returns>
        public TResource Update<TResource>(string path, TResource resource, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);

            DispatchRequest<TResource>(new OctopusRequest("PUT", uri, requestResource: resource), false);
            return DispatchRequest<TResource>(new OctopusRequest("GET", uri), true).ResponseResource;
        }

        /// <summary>
        /// Fetches raw content from the resource at the specified path, using the GET verb.
        /// </summary>
        /// <exception cref="OctopusSecurityException">
        /// HTTP 401 or 403: Thrown when the current user's API key was not valid, their
        /// account is disabled, or they don't have permission to perform the specified action.
        /// </exception>
        /// <exception cref="OctopusServerException">
        /// If any other error is successfully returned from the server (e.g., a 500
        /// server error).
        /// </exception>
        /// <exception cref="OctopusValidationException">HTTP 400: If there was a problem with the request provided by the user.</exception>
        /// <exception cref="OctopusResourceNotFoundException">HTTP 404: If the specified resource does not exist on the server.</exception>
        /// <param name="path">The path to the resource to fetch.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        /// <returns>A stream containing the content of the resource.</returns>
        public Stream GetContent(string path, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);
            return DispatchRequest<Stream>(new OctopusRequest("GET", uri), true).ResponseResource;
        }

        /// <summary>
        /// Creates or updates the raw content of the resource at the specified path, using the PUT verb.
        /// </summary>
        /// <param name="path">The path to the resource to create or update.</param>
        /// <param name="contentStream">A stream containing content of the resource.</param>
        public void PutContent(string path, Stream contentStream)
        {
            if (contentStream == null) throw new ArgumentNullException("contentStream");
            var uri = QualifyUri(path);
            DispatchRequest<Stream>(new OctopusRequest("PUT", uri, requestResource: contentStream), false);
        }

        public Uri QualifyUri(string path, object parameters = null)
        {
            var dictionary = parameters as IDictionary<string, object>;

            path = (dictionary == null) ? UrlTemplate.Resolve(path, parameters) : UrlTemplate.Resolve(path, dictionary);

            return serverEndpoint.OctopusServer.Resolve(path);
        }

        protected virtual OctopusResponse<TResponseResource> DispatchRequest<TResponseResource>(OctopusRequest request, bool readResponse)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(request.Uri);
            if (serverEndpoint.Proxy != null)
            {
                webRequest.Proxy = serverEndpoint.Proxy;
            }
            webRequest.CookieContainer = cookieContainer;
            webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json";
            webRequest.ReadWriteTimeout = ApiConstants.DefaultClientRequestTimeout;
            webRequest.Timeout = ApiConstants.DefaultClientRequestTimeout;
            webRequest.Credentials = serverEndpoint.Credentials ?? CredentialCache.DefaultNetworkCredentials;
            webRequest.Method = request.Method;
            webRequest.Headers[ApiConstants.ApiKeyHttpHeaderName] = serverEndpoint.ApiKey;
            webRequest.UserAgent = $"{ApiConstants.OctopusUserAgentProductName}/{clientVersion.ToNormalizedString()}";

            if (webRequest.Method == "PUT")
            {
                webRequest.Headers["X-HTTP-Method-Override"] = "PUT";
                webRequest.Method = "POST";
            }

            if (webRequest.Method == "DELETE")
            {
                webRequest.Headers["X-HTTP-Method-Override"] = "DELETE";
                webRequest.Method = "POST";
            }

            if (lazyRepository.IsValueCreated)
            {
                var expectedCookieName = $"{ApiConstants.AntiforgeryTokenCookiePrefix}_{Repository.RootDocument.InstallationId}";
                var antiforgeryCookie = cookieContainer.GetCookies(cookieOriginUri)
                    .Cast<Cookie>()
                    .SingleOrDefault(c => string.Equals(c.Name, expectedCookieName));
                if (antiforgeryCookie != null)
                {
                    webRequest.Headers[ApiConstants.AntiforgeryTokenHttpHeaderName] = antiforgeryCookie.Value;
                }
            }

            SendingOctopusRequest?.Invoke(request);

            BeforeSendingHttpRequest?.Invoke(webRequest);

            HttpWebResponse webResponse = null;

            try
            {
                if (request.RequestResource == null)
                {
                    webRequest.ContentLength = 0;
                }
                else
                {
                    var requestStreamContent = request.RequestResource as Stream;
                    if (requestStreamContent != null)
                    {
                        webRequest.Accept = null;
                        webRequest.ContentType = "application/octet-stream";
                        webRequest.ContentLength = requestStreamContent.Length;
                        requestStreamContent.CopyTo(webRequest.GetRequestStream());
                        // Caller owns stream.
                    }
                    else
                    {
                        var fileUploadContent = request.RequestResource as FileUpload;
                        if (fileUploadContent != null)
                        {
                            webRequest.AllowWriteStreamBuffering = false;
                            webRequest.SendChunked = true;

                            var boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
                            var boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

                            var requestStream = webRequest.GetRequestStream();
                            requestStream.Write(boundarybytes, 0, boundarybytes.Length);

                            var headerTemplate = "Content-Disposition: form-data; filename=\"{0}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                            var header = string.Format(headerTemplate, fileUploadContent.FileName);
                            var headerbytes = Encoding.UTF8.GetBytes(header);
                            requestStream.Write(headerbytes, 0, headerbytes.Length);
                            fileUploadContent.Contents.CopyTo(requestStream);
                            requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                            requestStream.Flush();
                            requestStream.Close();
                        }
                        else
                        {
                            var text = JsonConvert.SerializeObject(request.RequestResource, defaultJsonSerializerSettings);
                            webRequest.ContentLength = Encoding.UTF8.GetByteCount(text);
                            var requestStream = new StreamWriter(webRequest.GetRequestStream());
                            requestStream.Write(text);
                            requestStream.Flush();
                        }
                    }
                }

                webResponse = (HttpWebResponse)webRequest.GetResponse();
                AfterReceivingHttpResponse?.Invoke(webResponse);

                var resource = default(TResponseResource);
                if (readResponse)
                {
                    var responseStream = webResponse.GetResponseStream();
                    if (responseStream != null)
                    {
                        if (typeof(TResponseResource) == typeof(Stream))
                        {
                            var stream = new MemoryStream();
                            responseStream.CopyTo(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            resource = (TResponseResource)(object)stream;
                        }
                        else if (typeof(TResponseResource) == typeof(byte[]))
                        {
                            var stream = new MemoryStream();
                            responseStream.CopyTo(stream);
                            resource = (TResponseResource)(object)stream.ToArray();
                        }
                        else if (typeof(TResponseResource) == typeof(string))
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                resource = (TResponseResource)(object)reader.ReadToEnd();
                            }
                        }
                        else
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                var content = reader.ReadToEnd();
                                try
                                {
                                    resource = JsonConvert.DeserializeObject<TResponseResource>(content, defaultJsonSerializerSettings);
                                }
                                catch (Exception ex)
                                {
                                    throw new OctopusDeserializationException((int)webResponse.StatusCode, "Unable to process response from server: " + ex.Message + ". Response content: " + (content.Length > 100 ? content.Substring(0, 100) : content), ex);
                                }
                            }
                        }
                    }
                }

                var locationHeader = webResponse.Headers.Get("Location");
                var octopusResponse = new OctopusResponse<TResponseResource>(request, webResponse.StatusCode, locationHeader, resource);
                ReceivedOctopusResponse?.Invoke(octopusResponse);

                return octopusResponse;
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    throw OctopusExceptionFactory.CreateException(wex, (HttpWebResponse)wex.Response);
                }

                throw;
            }
            finally
            {
                if (webResponse != null)
                {
                    try
                    {
                        webResponse.Close();
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }
                }
            }
        }

        IOctopusRepository LoadRepository()
        {
            return new OctopusRepository(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
#endif