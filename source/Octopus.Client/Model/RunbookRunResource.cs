using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RunbookRunResource : Resource, IExecutionResource, IHaveSpaceResource
    {
        public RunbookRunResource()
        {
            FormValues = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        [Required(ErrorMessage = "Please specify the Runbook Snapshot to run.")]
        [WriteableOnCreate]
        public string RunbookSnapshotId { get; set; }

        public string FrozenRunbookProcessId { get; set; }

        [Required(ErrorMessage = "Please specify the Runbook to run.")]
        [WriteableOnCreate]
        public string RunbookId { get; set; }

        [Required(ErrorMessage = "Please provide a target environment to run to.")]
        [WriteableOnCreate]
        public string EnvironmentId { get; set; }

        [WriteableOnCreate]
        public string TenantId { get; set; }

        [WriteableOnCreate]
        public bool ForcePackageDownload { get; set; }

        [WriteableOnCreate]
        public bool ForcePackageRedeployment { get; set; }

        [WriteableOnCreate]
        public ReferenceCollection SkipActions { get; set; }

        /// <summary>
        /// A collection of machines in the target environment
        /// that should be deployed to. If the collection is
        /// empty, all enabled machines are deployed.
        /// </summary>
        [WriteableOnCreate]
        public ReferenceCollection SpecificMachineIds { get; set; }

        /// <summary>
        /// A collection of machines in the target environment that should be excluded from the deployment.
        /// </summary>
        [WriteableOnCreate]
        public ReferenceCollection ExcludedMachineIds { get; set; }

        public string ManifestVariableSetId { get; set; }
        public string TaskId { get; set; }
        public string ProjectId { get; set; }

        /// <summary>
        /// If set to true, the deployment will prompt for manual intervention (Fail/Retry/Ignore) when
        /// failures are encountered in activities that support it. May be overridden with the
        /// Octopus.UseGuidedFailure special variable.
        /// </summary>
        [WriteableOnCreate]
        public bool UseGuidedFailure { get; set; }

        [WriteableOnCreate]
        public string Comments { get; set; }

        [WriteableOnCreate]
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public Dictionary<string, string> FormValues { get; set; }

        /// <summary>
        /// If set this time will be the used to schedule the deployment to a later time, null is assumed to mean the time will
        /// be executed immediately.
        /// </summary>
        public DateTimeOffset? QueueTime { get; set; }

        public DateTimeOffset? QueueTimeExpiry { get; set; }

        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }

        public string SpaceId { get; set; }

        public RetentionPeriod TentacleRetentionPeriod { get; set; }

        public string DeployedBy { get; set; }

        public string DeployedById { get; set; }

        public bool FailureEncountered { get; set; }

        public ReferenceCollection DeployedToMachineIds { get; set; }
    }
}