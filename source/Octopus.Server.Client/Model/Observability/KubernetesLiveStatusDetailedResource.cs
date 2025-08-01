using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace Octopus.Client.Model.Observability;

public class KubernetesLiveStatusDetailedResource
{
    [Required]
    public string Name { get; set; }

    public string Namespace { get; set; }

    [Required]
    public string Kind { get; set; }

    [Required]
    public string HealthStatus { get; set; }

    public string SyncStatus { get; set; }

    [Required]
    public string MachineId { get; set; }

    [Required]
    public Instant LastUpdated { get; set; }

    public ManifestSummaryResource ManifestSummary { get; set; }

    [Required]
    public IReadOnlyCollection<KubernetesLiveStatusDetailedResource> Children { get; set; }

    public Guid? DesiredResourceId { get; set; }

    public Guid? ResourceId { get; set; }
}