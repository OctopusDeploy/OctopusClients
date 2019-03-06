using Octopus.Client.Model.IssueTrackers;

namespace Octopus.Client.Model.PackageMetadata
{
    public class OctopusPackageMetadataGetResource : Resource
    {
        public string PackageId { get; set; }
        public string Version { get; set; }

        public string BuildEnvironment { get; set; }
        public string BuildNumber { get; set; }
        public string BuildLink { get; set; }
        public string VcsRoot { get; set; }

        public string IssueTrackerName { get; set; }
        public WorkItemLink[] WorkItems { get; set; }
    }
}