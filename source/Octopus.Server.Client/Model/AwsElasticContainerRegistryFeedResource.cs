using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class AwsElasticContainerRegistryFeedResource : FeedResource
    {
        public override FeedType FeedType => FeedType.AwsElasticContainerRegistry;

        [Writeable]
        public string Region { get; set; }

        [Trim]
        [Writeable]
        public string AccessKey { get; set; }

        [Trim, Writeable]
        public SensitiveValue SecretKey { get; set; }
    }
}