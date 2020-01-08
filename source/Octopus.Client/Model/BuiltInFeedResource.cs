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
}