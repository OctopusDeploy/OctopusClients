using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class MavenFeedResource : FeedResource
    {
        public override FeedType FeedType => FeedType.Maven;

        [Writeable]
        public int DownloadAttempts { get; set; } = 5;

        [Writeable]
        public int DownloadRetryBackoffSeconds { get; set; } = 10;
    }
}