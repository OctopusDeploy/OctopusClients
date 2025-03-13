#nullable enable
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class AwsElasticContainerRegistryFeedResource : FeedResource
    {
        public AwsElasticContainerRegistryFeedResource(string region, AwsElasticContainerRegistryAuthDetails authDetails)
        {
            Region = region;
            AuthDetails = authDetails;
        }

        public override FeedType FeedType => FeedType.AwsElasticContainerRegistry;

        [Writeable]
        public string Region { get; set; }

        [Trim, Writeable]
        public AwsElasticContainerRegistryAuthDetails AuthDetails { get; set; }
    }

    public enum FeedAuthType
    {
        Key = 0,
        Oidc = 1,
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
    
    public class AwsElasticContainerRegistryAuthDetails : IOidcFeedResource
    {
        [Writeable] public FeedAuthType AuthType { get; set; } = FeedAuthType.Key;
        
        [Writeable]
        public AwsElasticContainerRegistryKeyAuthentication? KeyAuthentication { get; set; }

        [Writeable]
        public OidcFeedAuthentication? OidcAuthentication { get; set; }
    }
}