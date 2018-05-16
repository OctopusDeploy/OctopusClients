using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class NuGetFeedResource : FeedResource
    {
        public override FeedType FeedType => FeedType.NuGet;
        
        [Writeable]
        public int DownloadAttempts { get; set; }

        [Writeable]
        public int DownloadRetryBackoffSeconds { get; set; }

        [Writeable]
        public bool EnhancedMode { get; set; }
    }
}
