using System;
using System.Collections;
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Octopus.Client
{
    /// <summary>
    /// The Octopus Deploy RESTful HTTP API client.
    /// </summary>
    public class OctopusClient : IHttpOctopusClient
    {
        readonly object rootDocumentLock = new object();
        RootResource rootDocument;
        readonly OctopusServerEndpoint serverEndpoint;
        readonly JsonSerializerSettings defaultJsonSerializerSettings = JsonSerialization.GetDefaultSerializerSettings();
        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusClient" /> class.
        /// </summary>
        /// <param name="serverEndpoint">The server endpoint.</param>
        public OctopusClient(OctopusServerEndpoint serverEndpoint)
        {
            this.serverEndpoint = serverEndpoint;
            var handler = new HttpClientHandler()
            {
#if COREFX_ISSUE_11266_EXISTS
                Credentials = serverEndpoint.Credentials ==  CredentialCache.DefaultNetworkCredentials ? null : serverEndpoint.Credentials
#else
                Credentials = serverEndpoint.Credentials ?? CredentialCache.DefaultNetworkCredentials,
#endif
            };

            if (serverEndpoint.Proxy != null)
                handler.Proxy = serverEndpoint.Proxy;

            this.client = new HttpClient(handler, true);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(ApiConstants.ApiKeyHttpHeaderName, serverEndpoint.ApiKey);
        }

        /// <summary>
        /// Gets a document that identifies the Octopus server (from /api) and provides links to the resources available on the
        /// server. Instead of hardcoding paths,
        /// clients should use these link properties to traverse the resources on the server. This document is lazily loaded so
        /// that it is only requested once for
        /// the current <see cref="IOctopusClient" />.
        /// </summary>
        public RootResource RootDocument
        {
            get
            {
                if (rootDocument != null) return rootDocument;

                lock (rootDocumentLock)
                {
                    if (rootDocument != null) return rootDocument;

                    var root = EstablishSession();
                    Interlocked.MemoryBarrier();
                    rootDocument = root;
                    return root;
                }
            }
        }

        /// <summary>
        /// Requests a fresh root document from the Octopus Server which can be useful if the API surface has changed. This can occur when enabling/disabling features, or changing license.
        /// </summary>
        /// <returns>A fresh copy of the root document.</returns>
        public RootResource RefreshRootDocument()
        {
            var root = Get<RootResource>("~/api");
            lock (rootDocumentLock)
            {
                rootDocument = root;
            }
            return rootDocument;
        }

        /// <summary>
        /// Occurs when a request is about to be sent.
        /// </summary>
        public event Action<HttpRequestMessage> BeforeSendingHttpRequest;

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
            RootDocument.Link("Self");
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
        /// <returns>A stream containing the content of the resource.</returns>
        public Stream GetContent(string path)
        {
            var uri = QualifyUri(path);
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

        RootResource EstablishSession()
        {
            RootResource server;

            var watch = Stopwatch.StartNew();
            Exception lastError = null;

            // 60 second limit using Stopwatch alone makes debugging impossible.
            var retries = 3;

            while (true)
            {
                if (retries <= 0 && TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds) > TimeSpan.FromSeconds(60))
                {
                    if (lastError == null)
                    {
                        throw new Exception("Unable to connect to the Octopus Deploy server.");
                    }

                    throw new Exception("Unable to connect to the Octopus Deploy server. See the inner exception for details.", lastError);
                }

                try
                {
                    server = Get<RootResource>("~/api");
                    break;
                }
                catch (WebException ex)
                {
                    Thread.Sleep(1000);
                    lastError = ex;
                }
                catch (OctopusServerException ex)
                {
                    if (ex.HttpStatusCode != 503)
                    {
                        // 503 means the service is starting, so give it some time to start
                        throw;
                    }

                    Thread.Sleep(500);
                    lastError = ex;
                }
                retries--;
            }

            if (string.IsNullOrWhiteSpace(server.ApiVersion))
                throw new UnsupportedApiVersionException("This Octopus Deploy server uses an older API specification than this tool can handle. Please check for updates to the Octo tool.");

            var min = SemanticVersion.Parse(ApiConstants.SupportedApiSchemaVersionMin);
            var max = SemanticVersion.Parse(ApiConstants.SupportedApiSchemaVersionMax);
            var current = SemanticVersion.Parse(server.ApiVersion);

            if (current < min || current > max)
                throw new UnsupportedApiVersionException(string.Format("This Octopus Deploy server uses a newer API specification ({0}) than this tool can handle ({1} to {2}). Please check for updates to this tool.", server.ApiVersion, ApiConstants.SupportedApiSchemaVersionMin, ApiConstants.SupportedApiSchemaVersionMax));

            return server;
        }

        private OctopusResponse<TResponseResource> DispatchRequest<TResponseResource>(OctopusRequest request, bool readResponse)
        {
            return DispatchRequestAsync<TResponseResource>(request, readResponse).GetAwaiter().GetResult();
        }

        private async Task<OctopusResponse<TResponseResource>> DispatchRequestAsync<TResponseResource>(OctopusRequest request, bool readResponse)
        {
            using (var message = new HttpRequestMessage())
            {
                message.RequestUri = request.Uri;
                message.Method = new HttpMethod(request.Method);

                if (request.Method == "PUT" || request.Method == "DELETE")
                {
                    message.Method = HttpMethod.Post;
                    message.Headers.Add("X-HTTP-Method-Override", request.Method);
                }

                var requestHandler = SendingOctopusRequest;
                requestHandler?.Invoke(request);

                var webRequestHandler = BeforeSendingHttpRequest;
                webRequestHandler?.Invoke(message);


                if (request.RequestResource != null)
                    message.Content = GetContent(request);

                var ct = new CancellationToken(); // TODO
                var completionOption = readResponse
                    ? HttpCompletionOption.ResponseContentRead
                    : HttpCompletionOption.ResponseHeadersRead;

                using (var response = await client.SendAsync(message, completionOption, ct).ConfigureAwait(false))
                {
                    //   throw new TimeoutException($"Timeout after {ApiConstants.DefaultClientRequestTimeout}ms getting response");

                    if (!response.IsSuccessStatusCode)
                        throw await OctopusExceptionFactory.CreateException(response).ConfigureAwait(false);

                    var resource = readResponse
                        ? await ReadResponse<TResponseResource>(response).ConfigureAwait(false)
                        : default(TResponseResource);

                    var locationHeader = response.Headers.Location?.ToString();
                    var octopusResponse = new OctopusResponse<TResponseResource>(request, response.StatusCode, locationHeader, resource);
                    ReceivedOctopusResponse?.Invoke(octopusResponse);

                    return octopusResponse;
                }
            }
        }

        private HttpContent GetContent(OctopusRequest request)
        {
            var requestStreamContent = request.RequestResource as Stream;
            if (requestStreamContent != null)
            {
                var streamContent = new StreamContent(requestStreamContent);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return streamContent;
            }

            var fileUploadContent = request.RequestResource as FileUpload;
            if (fileUploadContent != null)
            {
                var formContent = new MultipartFormDataContent();
                var streamContent = new StreamContent(fileUploadContent.Contents);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                formContent.Add(streamContent, "file", fileUploadContent.FileName);
                return formContent;
            }

            var text = JsonConvert.SerializeObject(request.RequestResource, defaultJsonSerializerSettings);

            var content = new StringContent(text, Encoding.UTF8, "application/json");
            return content;
        }

        private async Task<T> ReadResponse<T>(HttpResponseMessage response)
        {
            var content = response.Content;

            if (typeof(T) == typeof(Stream))
            {
                var stream = new MemoryStream();
                await content.CopyToAsync(stream).ConfigureAwait(false);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)(object)stream;
            }

            if (typeof(T) == typeof(byte[]))
                return (T)(object)await content.ReadAsByteArrayAsync().ConfigureAwait(false);

            var str = await content.ReadAsStringAsync().ConfigureAwait(false);
            if (typeof(T) == typeof(string))
                return (T)(object)str;

            try
            {
                var s = str;
                return JsonConvert.DeserializeObject<T>(str, defaultJsonSerializerSettings);
            }
            catch (Exception ex)
            {
                throw new OctopusDeserializationException((int)response.StatusCode,
                    $"Unable to process response from server: {ex.Message}. Response content: {(str.Length > 1000 ? str.Substring(0, 1000) : str)}", ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            client?.Dispose();
        }
    }
}