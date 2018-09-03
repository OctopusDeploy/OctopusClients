using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class DockerFeedResource : FeedResource
    {
        public override FeedType FeedType => FeedType.Docker;

        [Writeable]
        public string ApiVersion { get; set; }

        [Writeable]
        public string RegistryPath { get; set; }
        
        [Writeable]
        public string FeedUri { get; set; }

        [Writeable]
        public string Username { get; set; }

        [Writeable]
        public SensitiveValue Password { get; set; }
    }
}