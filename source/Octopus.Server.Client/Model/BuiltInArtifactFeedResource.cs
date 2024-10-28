using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class BuiltInArtifactFeedResource : FeedResource
    {
        public override FeedType FeedType => FeedType.Artifact;
        
    }
}