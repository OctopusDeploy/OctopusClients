using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class ArtifactoryGenericFeedResource : FeedResource
    {
        public override FeedType FeedType => FeedType.ArtifactoryGeneric;

        [Writeable]
        public string FeedUri { get; set; }

        [Trim]
        [Writeable]
        public string LayoutRegex { get; set; }
        
        [Trim]
        [Writeable]
        public string Repository { get; set; }

        [Trim]
        [Writeable]
        public SensitiveValue Password { get; set; }

        [Trim]
        [Writeable]
        public string Username { get; set; }
    }
}
