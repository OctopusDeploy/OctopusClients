using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client
{
    /// <summary>
    /// Credentials for authenticating via OIDC token exchange. The client will call
    /// <see cref="OidcTokenProvider"/> to obtain an OIDC JWT, exchange it for an Octopus
    /// access token at <c>POST /token/v1</c>, and use the result as a Bearer token.
    /// The access token is cached and refreshed automatically before expiry.
    /// </summary>
    public class OidcTokenExchangeCredentials
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OidcTokenExchangeCredentials"/>.
        /// </summary>
        /// <param name="audience">
        /// The service account identification token (a UUID). This must match the <c>aud</c> claim
        /// in the OIDC JWT and is used to identify the Octopus service account to authenticate as.
        /// </param>
        /// <param name="oidcTokenProvider">
        /// A delegate that returns a fresh OIDC JWT when invoked. The client calls this whenever
        /// a token exchange or refresh is needed.
        /// </param>
        public OidcTokenExchangeCredentials(string audience, Func<CancellationToken, Task<string>> oidcTokenProvider)
        {
            if (string.IsNullOrWhiteSpace(audience))
                throw new ArgumentException("Audience is required.", nameof(audience));

            Audience = audience;
            OidcTokenProvider = oidcTokenProvider ?? throw new ArgumentNullException(nameof(oidcTokenProvider));
        }

        /// <summary>
        /// The service account identification token (UUID) used as the audience when exchanging tokens.
        /// </summary>
        public string Audience { get; }

        /// <summary>
        /// A delegate that provides a fresh OIDC JWT for token exchange.
        /// </summary>
        public Func<CancellationToken, Task<string>> OidcTokenProvider { get; }
    }
}
