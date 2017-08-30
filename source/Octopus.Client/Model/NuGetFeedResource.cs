namespace Octopus.Client.Model
{
#pragma warning disable 618
    public class NuGetFeedResource : FeedResource
#pragma warning restore 618
    {
        public override FeedType FeedType => FeedType.NuGet;
        
        [Writeable]
        public bool EnhancedMode { get; set; }
    }
}
