#nullable enable
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class GcsStorageFeedResource : FeedResource
    {
        public override FeedType FeedType => FeedType.GcsStorage;

        [Writeable]
        public int DownloadAttempts { get; set; } = 5;

        [Writeable]
        public int DownloadRetryBackoffSeconds { get; set; } = 10;

        [Writeable]
        public bool UseServiceAccountKey { get; set; }

        [Writeable]
        public SensitiveValue? ServiceAccountJsonKey { get; set; }

        [Writeable]
        public string? Project { get; set; }

        [Writeable]
        public GoogleOidcFeedAuthentication? OidcAuthentication { get; set; }
    }
}
