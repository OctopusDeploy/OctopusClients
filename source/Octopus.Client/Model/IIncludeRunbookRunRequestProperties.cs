using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public interface IIncludeRunbookRunRequestProperties
    {
        string EnvironmentId { get; set; }
        string TenantId { get; set; }
        bool ForcePackageDownload { get; set; }
        bool ForcePackageRedeployment { get; set; }
        ReferenceCollection SkipActions { get; set; }
        ReferenceCollection SpecificMachineIds { get; set; }
        ReferenceCollection ExcludedMachineIds { get; set; }
        bool UseGuidedFailure { get; set; }
        string Comments { get; set; }
        Dictionary<string, string> FormValues { get; set; }
        DateTimeOffset? QueueTime { get; set; }
        DateTimeOffset? QueueTimeExpiry { get; set; }
        RetentionPeriod TentacleRetentionPeriod { get; set; }
    }
}
