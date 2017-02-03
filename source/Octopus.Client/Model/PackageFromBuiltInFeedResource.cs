namespace Octopus.Client.Model
{
    public class PackageFromBuiltInFeedResource : PackageResource
    {
        public long? PackageSizeBytes { get; set; }
        public string Hash { get; set; }
    }
}