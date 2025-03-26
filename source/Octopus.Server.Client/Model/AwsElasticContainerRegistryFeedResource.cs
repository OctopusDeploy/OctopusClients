#nullable enable
using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class AwsElasticContainerRegistryFeedResource : FeedResource
    {
        public AwsElasticContainerRegistryFeedResource(string region, string? roleArn, AwsElasticContainerRegistryAuthDetails authDetails)
        {
            Region = region;
            RoleArn = roleArn;
            AuthDetails = authDetails;
        }

        public override FeedType FeedType => FeedType.AwsElasticContainerRegistry;

        [Writeable]
        public string Region { get; set; }
        
        [Writeable]
        public string? RoleArn { get; set; }

        [Trim, Writeable]
        public AwsElasticContainerRegistryAuthDetails AuthDetails { get; set; }
    }

    public static class FeedAuthType
    {
        public static readonly string Key = "Key";
        public static readonly string Oidc = "Oidc";
    }

    public class AwsElasticContainerRegistryKeyAuthentication
    {
        [Trim]
        [Writeable]
        public string? AccessKey { get; set; }

        [Trim]
        [Writeable]
        public SensitiveValue? SecretKey { get; set; }
    }

    public class AwsElasticContainerRegistryAuthDetails
    {
        [Writeable] public string AuthType { get; set; } = FeedAuthType.Key;
        
        [Writeable]
        public AwsElasticContainerRegistryKeyAuthentication? KeyAuthentication { get; set; }

        [Writeable]
        public EcrOidcFeedAuthentication? OidcAuthentication { get; set; }
    }

    public class EcrOidcFeedAuthentication : IOidcFeedAuthentication
    {
        public string? SessionDuration { get; set; } = "3600";
        public string? Audience { get; set; }
        public IEnumerable<string> SubjectKeys { get; set; } = [];
    }
}