using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class BuiltInFeedResource : FeedResource
    {
        public override FeedType FeedType => FeedType.BuiltIn;

        [Writeable]
        public int? DeleteUnreleasedPackagesAfterDays { get; set; }

        [Writeable]
        public bool IsBuiltInRepoSyncEnabled { get; set; } = true;
    }

    public class NuGetFeedResource : FeedResource
    {
        public static readonly int DefaultDownloadAttempts = 5;
        public static readonly int DefaultDownloadRetryBackoffSeconds = 10;
        public static readonly bool DefaultEnhancedMode = false;

        public override FeedType FeedType => FeedType.NuGet;

        [Writeable]
        public int DownloadAttempts { get; set; } = DefaultDownloadAttempts;

        [Writeable]
        public int DownloadRetryBackoffSeconds { get; set; } = DefaultDownloadRetryBackoffSeconds;

        [Writeable]
        public bool EnhancedMode { get; set; } = DefaultEnhancedMode;
    }
}
