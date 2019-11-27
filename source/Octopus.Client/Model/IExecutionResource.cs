using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public interface IExecutionResource : IResource
    {
        string Name { get; set; }
        DateTimeOffset Created { get; set; }
        bool ForcePackageDownload { get; set; }
        string Comments { get; set; }
        ReferenceCollection SkipActions { get; set; }
        ReferenceCollection SpecificMachineIds { get; set; }
        ReferenceCollection ExcludedMachineIds { get; set; }
        bool UseGuidedFailure { get; set; }
        RetentionPeriod TentacleRetentionPeriod { get; set; }
        string EnvironmentId { get; set; }
        string TenantId { get; set; }
        string TaskId { get; set; }
        string ProjectId { get; set; }
        string ManifestVariableSetId { get; set; }
        string DeployedBy { get; set; }
        string DeployedById { get; set; }
        bool FailureEncountered { get; }
        ReferenceCollection DeployedToMachineIds { get; set; }
        Dictionary<string, string> FormValues { get; set; }
        DateTimeOffset? QueueTime { get; set; }
        DateTimeOffset? QueueTimeExpiry { get; set; }
    }
}
