using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class HelmFeedResource: FeedResource  {
        public override FeedType FeedType => FeedType.Helm;
        
        [Writeable]
        public string FeedUri { get; set; }

        [Writeable]
        public string Username { get; set; }

        [Writeable]
        public SensitiveValue Password { get; set; }
    }
}