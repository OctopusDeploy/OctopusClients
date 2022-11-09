using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class AzureContainerRegistryFeedResource : DockerFeedResource
    {
        public override FeedType FeedType => FeedType.AzureContainerRegistry;
        
    }
}