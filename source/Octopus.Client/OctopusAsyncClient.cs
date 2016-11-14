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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Octopus.Client.Logging;
using Octopus.Client.Util;

namespace Octopus.Client
{
    /// <summary>
    /// The Octopus Deploy RESTful HTTP API client.
    /// </summary>
    public class OctopusAsyncClient : IOctopusAsyncClient
    {
        private static readonly ILog Logger = LogProvider.For<OctopusAsyncClient>();

        readonly OctopusServerEndpoint serverEndpoint;
        readonly JsonSerializerSettings defaultJsonSerializerSettings = JsonSerialization.GetDefaultSerializerSettings();
        private readonly HttpClient client;
        private readonly bool ignoreSslErrors = false;



        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusAsyncClient" /> class.
        /// </summary>
        /// <param name="serverEndpoint">The server endpoint.</param>
        /// <param name="options">The <see cref="OctopusClientOptions" /> used to configure the behavour of the client, may be null.</param>
        OctopusAsyncClient(OctopusServerEndpoint serverEndpoint, OctopusClientOptions options)
        {
            options = options ?? new OctopusClientOptions();
            Repository = new OctopusAsyncRepository(this);

            this.serverEndpoint = serverEndpoint;
            var handler = new HttpClientHandler()
            {
                Credentials = serverEndpoint.Credentials ?? CredentialCache.DefaultNetworkCredentials,
            };

#if HTTP_CLIENT_SUPPORTS_SSL_OPTIONS
            handler.SslProtocols = options.SslProtocols;
            ignoreSslErrors = options.IgnoreSslErrors;
            handler.ServerCertificateCustomValidationCallback = IgnoreServerCertificateCallback;
#endif

            if (serverEndpoint.Proxy != null)
                handler.Proxy = serverEndpoint.Proxy;
            
            client = new HttpClient(handler, true);
            client.Timeout = options.Timeout;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(ApiConstants.ApiKeyHttpHeaderName, serverEndpoint.ApiKey);
        }

        private bool IgnoreServerCertificateCallback(HttpRequestMessage message, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
            {
                return true;
            }

            var warning = $@"The following certificate errors were encountered when establishing the HTTPS connection to the server: {errors}
Certificate subject name: {certificate.SubjectName.Name}
Certificate thumbprint:   {certificate.Thumbprint}";

            if (ignoreSslErrors)
            {
                Logger.Warn(warning);
                Logger.Warn("Because --ignoreSslErrors was set, this will be ignored.");
                return true;
            }

            Logger.Error(warning);
            return false;
        }

        public static async Task<IOctopusAsyncClient> Create(OctopusServerEndpoint serverEndpoint, OctopusClientOptions options = null)
        {
            var client = new OctopusAsyncClient(serverEndpoint, options ?? new OctopusClientOptions());
            try
            {
                client.RootDocument = await client.EstablishSession().ConfigureAwait(false);
                return client;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Gets a document that identifies the Octopus server (from /api) and provides links to the resources available on the
        /// server. Instead of hardcoding paths,
        /// clients should use these link properties to traverse the resources on the server. This document is lazily loaded so
        /// that it is only requested once for
        /// the current <see cref="IOctopusAsyncClient" />.
        /// </summary>
        public RootResource RootDocument { get; private set; }

        /// <summary>
        /// Requests a fresh root document from the Octopus Server which can be useful if the API surface has changed. This can occur when enabling/disabling features, or changing license.
        /// </summary>
        /// <returns>A fresh copy of the root document.</returns>
        public async Task<RootResource> RefreshRootDocument()
        {
            RootDocument = await Get<RootResource>("~/api").ConfigureAwait(false);
            return RootDocument;
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
        /// Fetches a single resource from the server using the HTTP GET verb.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="path">The path from which to fetch the resource.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        /// <returns>
        /// The resource from the server.
        /// </returns>
        public async Task<TResource> Get<TResource>(string path, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);

            var response = await DispatchRequest<TResource>(new OctopusRequest("GET", uri), true).ConfigureAwait(false);
            return response.ResponseResource;
        }

        public IOctopusAsyncRepository Repository { get; }

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
        public async Task<ResourceCollection<TResource>> List<TResource>(string path, object pathParameters = null)
        {
            return await Get<ResourceCollection<TResource>>(path, pathParameters).ConfigureAwait(false);
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
        public async Task Paginate<TResource>(string path, object pathParameters, Func<ResourceCollection<TResource>, bool> getNextPage)
        {
            var page = await List<TResource>(path, pathParameters).ConfigureAwait(true);

            while (getNextPage(page) && page.Items.Count > 0 && page.HasLink("Page.Next"))
            {
                page = await List<TResource>(page.Link("Page.Next")).ConfigureAwait(true);
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
        public Task Paginate<TResource>(string path, Func<ResourceCollection<TResource>, bool> getNextPage)
        {
            return Paginate(path, null, getNextPage);
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
        public async Task<TResource> Create<TResource>(string path, TResource resource, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);

            var response = await DispatchRequest<TResource>(new OctopusRequest("POST", uri, requestResource: resource), true).ConfigureAwait(true);
            return await Get<TResource>(response.Location).ConfigureAwait(true);
        }

        /// <summary>
        /// Sends a command to a resource at the given URI on the server using the POST verb.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="path">The path to the container resource.</param>
        /// <param name="resource">The resource to create.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        public Task Post<TResource>(string path, TResource resource, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);

            return DispatchRequest<TResource>(new OctopusRequest("POST", uri, requestResource: resource), false);
        }

        /// <summary>
        /// Sends a command to a resource at the given URI on the server using the POST verb.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="path">The path to the container resource.</param>
        /// <param name="resource">The resource to post.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        public async Task<TResponse> Post<TResource, TResponse>(string path, TResource resource, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);
            var response = await DispatchRequest<TResponse>(new OctopusRequest("POST", uri, requestResource: resource), true).ConfigureAwait(true);
            return response.ResponseResource;
        }

        /// <summary>
        /// Sends a command to a resource at the given URI on the server using the POST verb.
        /// </summary>
        /// <param name="path">The path to the container resource.</param>
        public Task Post(string path)
        {
            var uri = QualifyUri(path);

            return DispatchRequest<string>(new OctopusRequest("POST", uri), false);
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
        public Task Put<TResource>(string path, TResource resource)
        {
            var uri = QualifyUri(path);

            return DispatchRequest<TResource>(new OctopusRequest("PUT", uri, requestResource: resource), false);
        }

        /// <summary>
        /// Deletes the resource at the given URI from the server using a the DELETE verb.
        /// </summary>
        /// <param name="path">The path to the resource to delete.</param>
        /// <param name="pathParameters">If the <c>path</c> is a URI template, parameters to use for substitution.</param>
        public Task Delete(string path, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);

            return DispatchRequest<string>(new OctopusRequest("DELETE", uri), true);
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
        public async Task<TResource> Update<TResource>(string path, TResource resource, object pathParameters = null)
        {
            var uri = QualifyUri(path, pathParameters);

            await DispatchRequest<TResource>(new OctopusRequest("PUT", uri, requestResource: resource), false).ConfigureAwait(false);
            var result = await DispatchRequest<TResource>(new OctopusRequest("GET", uri), true).ConfigureAwait(false);
            return result.ResponseResource;
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
        public async Task<Stream> GetContent(string path)
        {
            var uri = QualifyUri(path);
            var response = await DispatchRequest<Stream>(new OctopusRequest("GET", uri), true).ConfigureAwait(false);
            return response.ResponseResource;
        }

        /// <summary>
        /// Creates or updates the raw content of the resource at the specified path, using the PUT verb.
        /// </summary>
        /// <param name="path">The path to the resource to create or update.</param>
        /// <param name="contentStream">A stream containing content of the resource.</param>
        public Task PutContent(string path, Stream contentStream)
        {
            if (contentStream == null) throw new ArgumentNullException("contentStream");
            var uri = QualifyUri(path);
            return DispatchRequest<Stream>(new OctopusRequest("PUT", uri, requestResource: contentStream), false);
        }

        public Uri QualifyUri(string path, object parameters = null)
        {
            var dictionary = parameters as IDictionary<string, object>;

            path = (dictionary == null) ? UrlTemplate.Resolve(path, parameters) : UrlTemplate.Resolve(path, dictionary);

            return serverEndpoint.OctopusServer.Resolve(path);
        }

        async Task<RootResource> EstablishSession()
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
                    server = await Get<RootResource>("~/api").ConfigureAwait(true);
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

        private async Task<OctopusResponse<TResponseResource>> DispatchRequest<TResponseResource>(OctopusRequest request, bool readResponse)
        {
#if COREFX_ISSUE_11456_EXISTS
            try
            {
#endif
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

                    var completionOption = readResponse
                        ? HttpCompletionOption.ResponseContentRead
                        : HttpCompletionOption.ResponseHeadersRead;
                    try
                    {
                        using (var response = await client.SendAsync(message, completionOption).ConfigureAwait(false))
                        {
                            if (!response.IsSuccessStatusCode)
                                throw await OctopusExceptionFactory.CreateException(response).ConfigureAwait(false);

                            var resource = readResponse
                                ? await ReadResponse<TResponseResource>(response).ConfigureAwait(false)
                                : default(TResponseResource);

                            var locationHeader = response.Headers.Location?.ToString();
                            var octopusResponse = new OctopusResponse<TResponseResource>(request, response.StatusCode,
                                locationHeader, resource);
                            ReceivedOctopusResponse?.Invoke(octopusResponse);

                            return octopusResponse;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        throw new TimeoutException($"Timeout getting response, client timeout is set to {client.Timeout}.");
                    }
                }
#if COREFX_ISSUE_11456_EXISTS
            }
            catch (HttpRequestException hre) when (hre.InnerException?.Message == "The operation identifier is not valid")
            {
                throw new OctopusSecurityException(401, "You must be logged in to perform this action. Please provide a valid API key or log in again.");
            }
#endif
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