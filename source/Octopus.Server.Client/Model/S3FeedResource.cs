using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class S3FeedResource : FeedResource
    {
        public override FeedType FeedType => FeedType.S3;

        [Trim]
        [Writeable]
        public string AccessKey { get; set; }

        [Trim]
        [Writeable]
        public SensitiveValue SecretKey { get; set; }
    }
}
