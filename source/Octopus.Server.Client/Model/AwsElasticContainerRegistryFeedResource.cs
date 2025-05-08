#nullable enable
using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class AwsElasticContainerRegistryFeedResource : FeedResource
    {
        public AwsElasticContainerRegistryFeedResource(string region)
        {
            Region = region;
        }

        public override FeedType FeedType => FeedType.AwsElasticContainerRegistry;

        [Writeable] public string Region { get; set; }

        [Trim] [Writeable] public string? AccessKey { get; set; }

        [Trim] [Writeable] public SensitiveValue? SecretKey { get; set; }

        [Writeable]
        public EcrOidcFeedAuthentication? OidcAuthentication { get; set; }
        
        [Writeable]
        public bool UseMachineCredentials { get; set; }

    }

    public class EcrOidcFeedAuthentication : IOidcFeedAuthentication
    {
        public EcrOidcFeedAuthentication(string roleArn)
        {
            RoleArn = roleArn;
        }

        public string? SessionDuration { get; set; } = "3600";
        public string RoleArn { get; set; }
        public string? Audience { get; set; }
        public IEnumerable<string> SubjectKeys { get; set; } = [];
    }
}