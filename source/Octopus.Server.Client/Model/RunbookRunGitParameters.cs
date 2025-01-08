using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class RunbookRunGitParameters
    {
        /// <summary>
        /// Parameter class for marshalling params between OctopusCLI and Octopus Server
        /// This class is used to facilitate backwards compatibility while extending /runbooks/{id}/run"
        /// </summary>
        public RunbookRunGitParameters()
        {

        }

        public string RunbookId { get; set; }
        public string ProjectId { get; set; }
        public string Notes { get; set; }

        public List<SelectedPackage> SelectedPackages { get; set; } = new();

        public List<SelectedGitResource> SelectedGitResources { get; set; } = new();

    }

    public class RunGitRunbookRun
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected RunGitRunbookRun()
        {
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public RunGitRunbookRun(string environmentId)
        {
            EnvironmentId = environmentId;
        }

        public string EnvironmentId { get; set; }

        public string TenantId { get; set; }

        public bool ForcePackageDownload { get; set; } = false;

        public string[] SkipActions { get; set; }

        public string[] SpecificMachineIds { get; set; }

        public string[] ExcludedMachineIds { get; set; }

        public bool UseGuidedFailure { get; set; } = false;

        public Dictionary<string, string> FormValues { get; set; } = new();

        public DateTimeOffset? QueueTime { get; set; }

        public DateTimeOffset? QueueTimeExpiry { get; set; }

        public string Comments { get; set; }
    }
}