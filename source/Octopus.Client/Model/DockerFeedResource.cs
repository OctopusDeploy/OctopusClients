using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
#pragma warning disable 618
    public class DockerFeedResource : FeedResource
#pragma warning restore 618
    {
        public override FeedType FeedType => FeedType.Docker;

        [Writeable]
        public string ApiVersion { get; set; }

        [Writeable]
        public string RegistryPath { get; set; }
    }
}