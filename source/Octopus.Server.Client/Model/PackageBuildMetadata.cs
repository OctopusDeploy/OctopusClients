namespace Octopus.Client.Model
{
    public class PackageBuildMetadata
    {
        public string PackageId { get; set; }
        public string Version { get; set; }
        public string BuildNumber { get; set; }
        public string BuildUrl { get; set; }
        public string VcsType { get; set; }
        public string VcsRoot { get; set; }
        public string VcsCommitNumber { get; set; }
    }
}