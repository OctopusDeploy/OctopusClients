using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class GitHubFeedResource : FeedResource
    {
        public override FeedType FeedType => FeedType.GitHub;

        [Writeable]
        public int DownloadAttempts { get; set; } = 5;

        [Writeable]
        public int DownloadRetryBackoffSeconds { get; set; } = 10;
    }
}
