using System;
using System.Net;

namespace Octopus.Client
{
    /// <summary>
    /// Specifies the location and credentials to use when communicating with an Octopus Deploy server.
    /// </summary>
    public class OctopusServerEndpoint
    {
        /// <summary>
        /// Create an instance with a Token to authenticate.
        /// </summary>
        public static OctopusServerEndpoint CreateWithToken(string octopusServerAddress, string token, ICredentials credentials = null)
        {
            return new OctopusServerEndpoint(octopusServerAddress, new TokenValue(token), credentials);
        }

        /// <summary>
        /// Create an instance with an API Key to authenticate.
        /// </summary>
        public static OctopusServerEndpoint CreateWithApiKey(string octopusServerAddress, string apiKey, ICredentials credentials = null)
        {
            return new OctopusServerEndpoint(octopusServerAddress, apiKey, credentials);
        }

        private OctopusServerEndpoint(string octopusServerAddress, TokenValue token, ICredentials credentials)
        {
            if (string.IsNullOrWhiteSpace(token.Value))
                throw new ArgumentException("Token is required.");

            OctopusServer = GetLinkResolverFromServerUrl(octopusServerAddress);
            Token = token.Value;
            Credentials = credentials;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusServerEndpoint" /> class. Since no API key is provided, only
        /// very limited functionality will be available.
        /// </summary>
        /// <param name="octopusServerAddress">
        /// The URI of the Octopus Server. Ideally this should end with <c>/api</c>. If it ends with any other segment, the
        /// client
        /// will assume Octopus runs under a virtual directory.
        /// </param>
        public OctopusServerEndpoint(string octopusServerAddress)
        {
            OctopusServer = GetLinkResolverFromServerUrl(octopusServerAddress);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusServerEndpoint" /> class.
        /// </summary>
        /// <param name="octopusServerAddress">
        /// The URI of the Octopus Server. Ideally this should end with <c>/api</c>. If it ends with any other segment, the
        /// client
        /// will assume Octopus runs under a virtual directory.
        /// </param>
        /// <param name="apiKey">
        /// The API key to use when connecting to the Octopus Server. For more information on API keys, please
        /// see the API documentation on authentication
        /// (https://github.com/OctopusDeploy/OctopusDeploy-Api/blob/master/sections/authentication.md).
        /// </param>
        public OctopusServerEndpoint(string octopusServerAddress, string apiKey)
            : this(octopusServerAddress, apiKey, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusServerEndpoint" /> class.
        /// </summary>
        /// <param name="octopusServerAddress">
        /// The URI of the Octopus Server. Ideally this should end with <c>/api</c>. If it ends with any other segment, the
        /// client
        /// will assume Octopus runs under a virtual directory.
        /// </param>
        /// <param name="apiKey">
        /// The API key to use when connecting to the Octopus Server. For more information on API keys, please
        /// see the API documentation on authentication
        /// (https://github.com/OctopusDeploy/OctopusDeploy-Api/blob/master/sections/authentication.md).
        /// </param>
        /// <param name="credentials">
        /// Additional credentials to use when communicating to servers that require integrated/basic
        /// authentication.
        /// </param>
        public OctopusServerEndpoint(string octopusServerAddress, string apiKey, ICredentials credentials) : this(GetLinkResolverFromServerUrl(octopusServerAddress), apiKey, credentials ?? CredentialCache.DefaultNetworkCredentials)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusServerEndpoint" /> class.
        /// </summary>
        /// <param name="octopusServer">The resolver that should be used to turn relative links into full URIs.</param>
        /// <param name="apiKey">
        /// The API key to use when connecting to the Octopus Server. For more information on API keys, please
        /// see the API documentation on authentication
        /// (https://github.com/OctopusDeploy/OctopusDeploy-Api/blob/master/sections/authentication.md).
        /// </param>
        /// <param name="credentials">
        /// Additional credentials to use when communicating to servers that require integrated/basic
        /// authentication.
        /// </param>
        public OctopusServerEndpoint(ILinkResolver octopusServer, string apiKey, ICredentials credentials)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("An API key was not specified. Please set an API key using the ApiKey property. This can be gotten from your user profile page under the Octopus web portal.");

            OctopusServer = octopusServer;
            ApiKey = apiKey;
            Credentials = credentials ?? CredentialCache.DefaultNetworkCredentials;
        }

        /// <summary>
        /// The URI of the Octopus Server. Ideally this should end with <c>/api</c>. If it ends with any other segment, the
        /// client  will assume Octopus runs under a virtual directory.
        /// </summary>
        public ILinkResolver OctopusServer { get; }

        /// <summary>
        /// Indicates whether a secure (SSL) connection is being used to communicate with the server.
        /// </summary>
        public bool IsUsingSecureConnection => OctopusServer.IsUsingSecureConnection;

        /// <summary>
        /// Gets the API key to use when connecting to the Octopus Server. For more information on API keys, please see the API
        /// documentation on authentication
        /// (https://github.com/OctopusDeploy/OctopusDeploy-Api/blob/master/sections/authentication.md).
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// A JWT Token that can be used to authenticate calls to the Server.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Gets the additional credentials to use when communicating to servers that require integrated/basic authentication.
        /// </summary>
        public ICredentials Credentials { get; }

        /// <summary>
        /// Recreates the endpoint using the API key of a new user.
        /// </summary>
        /// <param name="newUserApiKey">The new user API key.</param>
        /// <returns>An endpoint with a new user.</returns>
        public OctopusServerEndpoint AsUser(string newUserApiKey)
        {
            return new OctopusServerEndpoint(OctopusServer, newUserApiKey, Credentials);
        }

        /// <summary>
        /// A proxy that should be used to connect to the endpoint.
        /// </summary>
        public IWebProxy Proxy { get; set; }

        private static DefaultLinkResolver GetLinkResolverFromServerUrl(string octopusServerAddress)
        {
            if (string.IsNullOrWhiteSpace(octopusServerAddress))
                throw new ArgumentException("An Octopus Server URI was not specified.");

            if (!Uri.TryCreate(octopusServerAddress, UriKind.Absolute, out var uri) || !uri.Scheme.StartsWith("http"))
                throw new ArgumentException($"The Octopus Server URI '{octopusServerAddress}' is invalid. The URI should start with http:// or https:// and be a valid URI.");

            return new DefaultLinkResolver(new Uri(octopusServerAddress));
        }

        private class TokenValue
        {
            public TokenValue(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }
    }


}
