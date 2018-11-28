using System;

namespace Octopus.Client.Model
{
    public class ApiConstants
    {
        public static readonly int DefaultClientRequestTimeout = 1000 * 60 * 10;
        public static readonly string SupportedApiSchemaVersionMin = "3.0.0";
        public static readonly string SupportedApiSchemaVersionMax = "3.0.99";
        public static readonly string AuthenticationCookiePrefix = "OctopusIdentificationToken";
        public static readonly string ApiKeyHttpHeaderName = "X-Octopus-ApiKey";
        public static readonly string AntiforgeryTokenCookiePrefix = "Octopus-Csrf-Token";
        public static readonly string AntiforgeryTokenHttpHeaderName = "X-Octopus-Csrf-Token";
        public static readonly string OctopusUserAgentProductName = "OctopusClient-dotnet";
    }
}