using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public abstract class ApiKeyResourceBase : Resource
    {
        [WriteableOnCreate]
        public string Purpose { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Expires { get; set; }
    }

    public class ApiKeyResource : ApiKeyResourceBase
    {
        public SensitiveValue ApiKey { get; set; }
    }

    public class ApiKeyCreatedResource : ApiKeyResourceBase
    {
        public string ApiKey { get; set; }
    }
}