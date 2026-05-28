using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client
{
    internal class OidcAuthenticationHandler : DelegatingHandler
    {
        readonly OidcAccessTokenCache cache;

        public OidcAuthenticationHandler(OidcAccessTokenCache cache, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            this.cache = cache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await cache.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
