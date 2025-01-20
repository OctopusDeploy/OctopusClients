using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Model
{
    public class RunGitRunbookParameters(GitRunbookRunParameters[] runs)
    {
        public GitRunbookRunParameters[] Runs { get; set; } = runs;
        public string Notes { get; set; }
        public List<SelectedPackage> SelectedPackages { get; set; } = new();
        public List<SelectedGitResource> SelectedGitResources { get; set; } = new();
    }

    public class GitRunbookRunParameters(string environmentId)
    {
        public string EnvironmentId { get; set; } = environmentId;

        public string TenantId { get; set; }

        public bool ForcePackageDownload { get; set; } = false;

        public string[] SkipActions { get; set; } = [];

        public string[] SpecificMachineIds { get; set; } = [];

        public string[] ExcludedMachineIds { get; set; } = [];

        public bool UseGuidedFailure { get; set; } = false;

        public Dictionary<string, string> FormValues { get; set; } = new();

        public DateTimeOffset? QueueTime { get; set; }

        public DateTimeOffset? QueueTimeExpiry { get; set; }

        public string Comments { get; set; }
    }
}