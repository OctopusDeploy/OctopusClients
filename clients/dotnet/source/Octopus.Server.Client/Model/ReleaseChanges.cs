using System.Collections.Generic;
using Octopus.Client.Model.BuildInformation;
using Octopus.Client.Model.IssueTrackers;

namespace Octopus.Client.Model
{
    public class ReleaseChanges
    {
        public string Version { get; set; }
        public string ReleaseNotes { get; set; }
        public List<ReleasePackageVersionBuildInformationResource> BuildInformation { get; set; }

        /// <summary>
        /// Aggregate of distinct work items from all VersionMetadata
        /// </summary>
        public List<WorkItemLink> WorkItems { get; set; }

        /// <summary>
        /// Aggregate of distinct commits from all VersionMetadata
        /// </summary>
        public List<CommitDetails> Commits { get; set; }
    }
}