using System;

namespace Octopus.Client.Model
{
    public class ApiConstants
    {
        public const int DefaultClientRequestTimeout = 1000*60*10;
        public const string SupportedApiSchemaVersionMin = "3.0.0";
        public const string SupportedApiSchemaVersionMax = "3.0.99";
        public const string ApiKeyHttpHeaderName = "X-Octopus-ApiKey";
    }
}