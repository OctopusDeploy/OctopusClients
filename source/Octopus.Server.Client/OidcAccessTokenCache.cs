using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Octopus.Client.Exceptions;

namespace Octopus.Client
{
    internal class OidcAccessTokenCache : IDisposable
    {
        internal static string WellKnownOpenIdConfigurationUrl = "~/.well-known/openid-configuration";
        static readonly TimeSpan ExpiryBuffer = TimeSpan.FromSeconds(30);

        readonly OidcTokenExchangeCredentials credentials;
        readonly Uri openIdConfigurationUri;
        readonly HttpClient httpClient;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        // volatile ensures the fast-path read outside the semaphore observes the latest write
        volatile string cachedToken;
        // Accessed via Interlocked.Read/Exchange for safe 64-bit reads on 32-bit targets
        long tokenExpiresAtTicks;
        // Discovered lazily on first refresh; written once while the semaphore is held
        Uri tokenEndpointUri;

        public OidcAccessTokenCache(OidcTokenExchangeCredentials credentials, Uri openIdConfigurationUri, HttpClient httpClient)
        {
            this.credentials = credentials;
            this.openIdConfigurationUri = openIdConfigurationUri;
            this.httpClient = httpClient;
        }

        public void Dispose()
        {
            semaphore.Dispose();
            httpClient.Dispose();
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            // Fast path: check without acquiring the semaphore
            if (cachedToken != null && DateTimeOffset.UtcNow.Ticks < Interlocked.Read(ref tokenExpiresAtTicks))
                return cachedToken;

            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // Double-check inside the lock
                if (cachedToken != null && DateTimeOffset.UtcNow.Ticks < Interlocked.Read(ref tokenExpiresAtTicks))
                    return cachedToken;

                return await RefreshAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Returns an access token synchronously by blocking on <see cref="GetAccessTokenAsync(CancellationToken)"/>.
        /// </summary>
        /// <remarks>
        /// This method performs a synchronous wait on asynchronous operations, including the user-supplied
        /// <c>OidcTokenProvider</c> delegate on <see cref="OidcTokenExchangeCredentials"/>. When used on a thread
        /// with a non-default <see cref="System.Threading.SynchronizationContext"/> (for example, UI or ASP.NET
        /// request threads), this can lead to deadlocks if the <c>OidcTokenProvider</c> implementation captures
        /// the calling context and resumes on it.
        ///
        /// To avoid deadlocks, any asynchronous implementation supplied via <c>OidcTokenProvider</c> must be
        /// written so that it does not depend on resuming on the original <see cref="System.Threading.SynchronizationContext"/>,
        /// and should use <c>ConfigureAwait(false)</c> on its own awaits where appropriate.
        /// </remarks>
        public string GetAccessToken()
            // GetAccessTokenAsync uses ConfigureAwait(false) throughout, so it does not
            // capture the caller's SynchronizationContext and is safe to block on here
            // without risking a sync-over-async deadlock or needing Task.Run indirection.
            => GetAccessTokenAsync(CancellationToken.None).GetAwaiter().GetResult();

        async Task<string> RefreshAsync(CancellationToken cancellationToken)
        {
            // Discover the token endpoint on first refresh; safe to write here as the semaphore is held
            if (tokenEndpointUri == null)
                tokenEndpointUri = await DiscoverTokenEndpointAsync(cancellationToken).ConfigureAwait(false);

            var oidcJwt = await credentials.OidcTokenProvider(cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(oidcJwt))
                throw new InvalidOperationException(
                    "The OIDC token provider returned a null or empty token. Ensure the provider is correctly configured and returns a valid JWT.");

            var requestBody = JsonConvert.SerializeObject(new
            {
                grant_type = "urn:ietf:params:oauth:grant-type:token-exchange",
                audience = credentials.Audience,
                subject_token = oidcJwt,
                subject_token_type = "urn:ietf:params:oauth:token-type:jwt"
            });

            // Use httpClient — must NOT go through the main client's handler chain,
            // which would cause infinite recursion via OidcAuthenticationHandler.
            using var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpointUri)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = TryParseOidcError(responseBody, response);
                var message = errorResponse != null
                    ? $"OIDC token exchange failed: {errorResponse.Error} – {errorResponse.ErrorDescription}"
                    : $"OIDC token exchange failed with HTTP {(int)response.StatusCode}: {responseBody}";
                throw new OctopusSecurityException((int)response.StatusCode, message);
            }

            var tokenResponse = JsonConvert.DeserializeObject<OidcTokenResponse>(responseBody);

            if (string.IsNullOrWhiteSpace(tokenResponse?.AccessToken))
                throw new OctopusServerException(
                    (int)response.StatusCode,
                    $"OIDC token exchange succeeded with HTTP {(int)response.StatusCode} but the response did not contain an access token: {responseBody}");

            cachedToken = tokenResponse.AccessToken;
            // When expires_in is shorter than ExpiryBuffer the effective lifetime is zero,
            // so the token is considered immediately expired and every call triggers a refresh.
            var effectiveLifetime = Math.Max(0, tokenResponse.ExpiresIn - (int)ExpiryBuffer.TotalSeconds);
            Interlocked.Exchange(ref tokenExpiresAtTicks, DateTimeOffset.UtcNow.AddSeconds(effectiveLifetime).Ticks);
            return cachedToken;
        }

        async Task<Uri> DiscoverTokenEndpointAsync(CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, openIdConfigurationUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new OctopusServerException(
                    (int)response.StatusCode,
                    $"Failed to retrieve OpenID Connect configuration from {openIdConfigurationUri}: HTTP {(int)response.StatusCode}");

            var config = JsonConvert.DeserializeObject<OpenIdConfigurationResponse>(responseBody);

            if (string.IsNullOrWhiteSpace(config?.TokenEndpoint))
                throw new InvalidOperationException(
                    $"The OpenID Connect configuration at {openIdConfigurationUri} did not contain a token_endpoint.");

            return new Uri(config.TokenEndpoint);
        }

        static OidcErrorResponse TryParseOidcError(string responseBody, HttpResponseMessage response)
        {
            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType == null || !contentType.Contains("json"))
                return null;

            try
            {
                return JsonConvert.DeserializeObject<OidcErrorResponse>(responseBody);
            }
            catch
            {
                return null;
            }
        }

        class OpenIdConfigurationResponse
        {
            [JsonProperty("token_endpoint")]
            public string TokenEndpoint { get; set; }
        }

        class OidcTokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("issued_token_type")]
            public string IssuedTokenType { get; set; }
        }

        class OidcErrorResponse
        {
            [JsonProperty("error")]
            public string Error { get; set; }

            [JsonProperty("error_description")]
            public string ErrorDescription { get; set; }
        }
    }
}
