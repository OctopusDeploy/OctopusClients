using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.LiveStatus;

public class KubernetesMonitorResource
{
    [Required]
    public string Id { get; set; }

    [Required]
    public Guid InstallationId { get; set; }

    [Required]
    public string MachineId { get; set; }

    [Required]
    public string SpaceId { get; set; }
}
