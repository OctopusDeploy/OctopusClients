using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.LiveStatus;

public class GetLiveKubernetesResourceManifestRequest
{
    [Required]
    public string ProjectId { get; set; }

    [Required]
    public string EnvironmentId { get; set; }

    public string TenantId { get; set; }

    [Required]
    public string SourceId { get; set; }

    [Required]
    public Guid DesiredOrKubernetesMonitoredResourceId { get; set; }
}
