using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability;

public class GetResourceRequest
{
    [Required]
    public string ProjectId { get; set; }

    [Required]
    public string EnvironmentId { get; set; }

    public string TenantId { get; set; }

    [Required]
    public string MachineId { get; set; }

    [Required]
    public Guid DesiredOrKubernetesMonitoredResourceId { get; set; }
}