using Octopus.Client.Model.IssueTrackers;

namespace Octopus.Client.Model.BuildInformation
{
    public class OctopusBuildInformationMappedResource : Resource
    {
        public string PackageId { get; set; }
        public string Version { get; set; }

        public string BuildEnvironment { get; set; }
        public string BuildNumber { get; set; }
        public string BuildUrl { get; set; }
        public string Branch { get; set; }
        public string VcsType { get; set; }
        public string VcsRoot { get; set; }
        public string VcsCommitNumber { get; set; }
        public string VcsCommitUrl { get; set; }

        public string IssueTrackerName { get; set; }
        public WorkItemLink[] WorkItems { get; set; }
        public CommitDetail[] Commits { get; set; }

        public string IncompleteDataWarning { get; set; }
    }
}