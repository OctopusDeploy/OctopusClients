using System;
using System.Net;

namespace Octopus.Client
{
    /// <summary>
    /// Specifies the location and credentials to use when communicating with an Octopus Deploy server.
    /// </summary>
    public class OctopusServerEndpoint
    {
        public static OctopusServerEndpoint CreateWithToken(string octopusServerAddress, string token)
        {
            return new OctopusServerEndpoint(octopusServerAddress, token, null, isToken: true);
        }

        public static OctopusServerEndpoint CreateWithApiKey(string octopusServerAddress, string apiKey)
        {
            return new OctopusServerEndpoint(octopusServerAddress, apiKey);
        }

        /// <remarks>
        /// Since no API key is provided, only very limited functionality will be available.
        /// </remarks>
        /// <param name="octopusServerAddress"><inheritdoc cref="OctopusServer" path="summary/"/></param>
        public OctopusServerEndpoint(string octopusServerAddress)
        {
            OctopusServer = ParseOctopusServerAddress(octopusServerAddress);
        }

        /// <param name="octopusServerAddress"><inheritdoc cref="OctopusServer" path="summary/"/></param>
        /// <param name="apiKey"><inheritdoc cref="ApiKey" path="summary/"/></param>
        public OctopusServerEndpoint(string octopusServerAddress, string apiKey)
            : this(octopusServerAddress, apiKey, null)
        {
        }

        /// <param name="octopusServerAddress"><inheritdoc cref="OctopusServer" path="summary/"/></param>
        /// <param name="apiKeyOrToken">
        /// API Key or Token used for authentication.
        /// See <see cref="ApiKey"/> and <see cref="Token"/>
        /// </param>
        /// <param name="credentials"><inheritdoc cref="Credentials" path="summary/"/></param>
        /// <param name="isToken">
        /// Indicates if <see langword="apiKeyOrToken"/> is a token or not. The default is <see langword="false"/>.
        /// </param>
        public OctopusServerEndpoint(string octopusServerAddress, string apiKeyOrToken, ICredentials credentials, bool isToken = false)
        : this(ParseOctopusServerAddress(octopusServerAddress), apiKeyOrToken, credentials, isToken)
        {
        }

        /// <param name="octopusServer"><inheritdoc cref="OctopusServer" path="summary/"/></param>
        /// <param name="apiKeyOrToken">
        /// API Key or Token used for authentication.
        /// See <see cref="ApiKey"/> and <see cref="Token"/>
        /// </param>
        /// <param name="credentials"><inheritdoc cref="Credentials" path="summary/"/></param>
        /// <param name="isToken">
        /// Indicates if <see langword="apiKeyOrToken"/> is a token or not. The default is <see langword="false"/>.
        /// </param>
        public OctopusServerEndpoint(ILinkResolver octopusServer, string apiKeyOrToken, ICredentials credentials, bool isToken = false)
        {
            if (string.IsNullOrWhiteSpace(apiKeyOrToken))
                throw new ArgumentException($"{(isToken ? "A Token" : "An API key")} was not specified but is required. If you This can be gotten from your user profile page under the Octopus web portal.");

            OctopusServer = octopusServer;
            ApiKey = !isToken ? apiKeyOrToken : null;
            Token = isToken ? apiKeyOrToken : null;
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
        /// The API key to use when connecting to the Octopus Server.
        ///
        /// For more information on API keys, please see the API documentation on authentication
        /// (https://github.com/OctopusDeploy/OctopusDeploy-Api/blob/master/sections/authentication.md).
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// The JWT Token to use when connecting to the Octopus Server.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// The additional credentials to use when communicating to servers that require integrated/basic authentication.
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

        private static DefaultLinkResolver ParseOctopusServerAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("An Octopus Server URI was not specified.");

            if (!Uri.TryCreate(address, UriKind.Absolute, out var uri) || !uri.Scheme.StartsWith("http"))
                throw new ArgumentException($"The Octopus Server URI '{address}' is invalid. The URI should start with http:// or https:// and be a valid URI.");

            return new DefaultLinkResolver(new Uri(address));
        }
    }
}
