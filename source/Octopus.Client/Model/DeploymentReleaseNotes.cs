using System.Collections.Generic;
using Octopus.Client.Model.IssueTrackers;

namespace Octopus.Client.Model
{
    public class DeploymentReleaseNotes
    {
        public string Version { get; set; }
        public string ReleaseNotes { get; set; }
        public List<WorkItemLink> WorkItems { get; set; }
    }
}