using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class GoogleContainerRegistryFeedResource : DockerFeedResource
    {
        public override FeedType FeedType => FeedType.GoogleContainerRegistry;
        
    }
}