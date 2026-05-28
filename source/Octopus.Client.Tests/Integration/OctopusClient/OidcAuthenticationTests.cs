using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Exceptions;
using HttpStatusCode = Nancy.HttpStatusCode;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class OidcAuthenticationTests : HttpIntegrationTestBase
    {
        // Static so they're shared across module instances (Nancy creates one per request)
        static int exchangeCallCount;
        static string lastReceivedOidcToken;
        static int tokenExpiresIn = 3600;
        static bool shouldFailExchange;

        public OidcAuthenticationTests()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            // Root-level well-known discovery endpoint — resolved via
            // DefaultLinkResolver.Resolve("~/.well-known/openid-configuration")
            Get($"{TestRootPath}/.well-known/openid-configuration", _ =>
                Response.AsJson(new
                {
                    issuer = HostBaseUri,
                    token_endpoint = $"{HostBaseUri}/{TestRootPath}/token/v1",
                    jwks_uri = $"{HostBaseUri}/{TestRootPath}/.well-known/jwks"
                }));

            // Root-level token exchange endpoint (URL discovered via openid-configuration)
            Post($"{TestRootPath}/token/v1", _ =>
            {
                Interlocked.Increment(ref exchangeCallCount);
                var body = Request.Body.AsString();
                var req = JsonConvert.DeserializeObject<TokenExchangeRequest>(body);
                lastReceivedOidcToken = req?.SubjectToken;

                if (shouldFailExchange)
                {
                    return Response.AsJson(new
                    {
                        error = "invalid_request",
                        error_description = "Subject token is invalid"
                    }, HttpStatusCode.BadRequest);
                }

                var accessToken = $"octopus-access-token-{exchangeCallCount}";
                return Response.AsJson(new
                {
                    access_token = accessToken,
                    issued_token_type = "urn:ietf:params:oauth:token-type:access_token",
                    token_type = "Bearer",
                    expires_in = tokenExpiresIn
                });
            });

            // Echoes back the Authorization header so tests can assert on it
            Get($"{TestRootPath}/echo-auth", _ =>
                Response.AsJson(new AuthEchoDto { Authorization = Request.Headers.Authorization }));
        }

        [SetUp]
        public override async Task Setup()
        {
            exchangeCallCount = 0;
            lastReceivedOidcToken = null;
            tokenExpiresIn = 3600;
            shouldFailExchange = false;
            // Don't call base.Setup() — each test creates its own OIDC client
            await Task.CompletedTask;
        }

        [TearDown]
        public override void TearDown()
        {
            // AsyncClient and SyncClient are null since we skip base.Setup()
            // Clients created per-test are disposed within each test using 'using'
        }

        // ── Async client tests ────────────────────────────────────────────────

        [Test]
        public async Task AsyncClient_InitialExchange_SetsAuthorizationHeader()
        {
            var oidcJwt = "fake-oidc-jwt";
            var endpoint = OctopusServerEndpoint.CreateWithOidcTokenExchange(
                HostBaseUri + TestRootPath,
                audience: "service-account-uuid",
                oidcTokenProvider: _ => Task.FromResult(oidcJwt));

            using var client = await OctopusAsyncClient.Create(endpoint).ConfigureAwait(false);

            var result = await client.Get<AuthEchoDto>($"{TestRootPath}/echo-auth").ConfigureAwait(false);

            result.Authorization.Should().StartWith("Bearer octopus-access-token-");
            exchangeCallCount.Should().Be(1);
            lastReceivedOidcToken.Should().Be(oidcJwt);
        }

        [Test]
        public async Task AsyncClient_TokenIsCachedAcrossMultipleRequests()
        {
            var endpoint = OctopusServerEndpoint.CreateWithOidcTokenExchange(
                HostBaseUri + TestRootPath,
                audience: "service-account-uuid",
                oidcTokenProvider: _ => Task.FromResult("jwt"));

            using var client = await OctopusAsyncClient.Create(endpoint).ConfigureAwait(false);

            await client.Get<AuthEchoDto>($"{TestRootPath}/echo-auth").ConfigureAwait(false);
            await client.Get<AuthEchoDto>($"{TestRootPath}/echo-auth").ConfigureAwait(false);
            await client.Get<AuthEchoDto>($"{TestRootPath}/echo-auth").ConfigureAwait(false);

            // One exchange during Create (LoadRootDocument), none for the three subsequent calls
            exchangeCallCount.Should().Be(1, "token should be cached after first exchange");
        }

        [Test]
        public async Task AsyncClient_RefreshesTokenWhenExpired()
        {
            // 0-second expiry: token is always past expiry relative to the 30s buffer,
            // so every request triggers a fresh exchange — no time-based waiting needed
            tokenExpiresIn = 0;

            var endpoint = OctopusServerEndpoint.CreateWithOidcTokenExchange(
                HostBaseUri + TestRootPath,
                audience: "service-account-uuid",
                oidcTokenProvider: _ => Task.FromResult("jwt"));

            using var client = await OctopusAsyncClient.Create(endpoint).ConfigureAwait(false);

            var first = await client.Get<AuthEchoDto>($"{TestRootPath}/echo-auth").ConfigureAwait(false);

            exchangeCallCount.Should().BeGreaterThanOrEqualTo(2, "each call after expiry should trigger a new exchange");
            first.Authorization.Should().StartWith("Bearer octopus-access-token-");
        }

        [Test]
        public async Task AsyncClient_ThrowsOctopusSecurityException_WhenExchangeFails()
        {
            shouldFailExchange = true;

            var endpoint = OctopusServerEndpoint.CreateWithOidcTokenExchange(
                HostBaseUri + TestRootPath,
                audience: "service-account-uuid",
                oidcTokenProvider: _ => Task.FromResult("jwt"));

            Func<Task> act = () => OctopusAsyncClient.Create(endpoint);
            await act.Should().ThrowAsync<OctopusSecurityException>()
                .WithMessage("*OIDC token exchange failed*").ConfigureAwait(false);
        }

        // ── Sync client tests ─────────────────────────────────────────────────

        [Test]
        public void SyncClient_InitialExchange_SetsAuthorizationHeader()
        {
            var oidcJwt = "fake-oidc-jwt";
            var endpoint = OctopusServerEndpoint.CreateWithOidcTokenExchange(
                HostBaseUri + TestRootPath,
                audience: "service-account-uuid",
                oidcTokenProvider: _ => Task.FromResult(oidcJwt));

            using var client = new Client.OctopusClient(endpoint);

            var result = client.Get<AuthEchoDto>($"{TestRootPath}/echo-auth");

            result.Authorization.Should().StartWith("Bearer octopus-access-token-");
            exchangeCallCount.Should().Be(1);
            lastReceivedOidcToken.Should().Be(oidcJwt);
        }

        [Test]
        public void SyncClient_TokenIsCachedAcrossMultipleRequests()
        {
            var endpoint = OctopusServerEndpoint.CreateWithOidcTokenExchange(
                HostBaseUri + TestRootPath,
                audience: "service-account-uuid",
                oidcTokenProvider: _ => Task.FromResult("jwt"));

            using var client = new Client.OctopusClient(endpoint);

            client.Get<AuthEchoDto>($"{TestRootPath}/echo-auth");
            client.Get<AuthEchoDto>($"{TestRootPath}/echo-auth");
            client.Get<AuthEchoDto>($"{TestRootPath}/echo-auth");

            exchangeCallCount.Should().Be(1, "token should be cached after first exchange");
        }

        // ── DTOs ──────────────────────────────────────────────────────────────

        public class AuthEchoDto
        {
            public string Authorization { get; set; }
        }

        class TokenExchangeRequest
        {
            [JsonProperty("grant_type")]
            public string GrantType { get; set; }

            [JsonProperty("audience")]
            public string Audience { get; set; }

            [JsonProperty("subject_token")]
            public string SubjectToken { get; set; }

            [JsonProperty("subject_token_type")]
            public string SubjectTokenType { get; set; }
        }
    }
}
