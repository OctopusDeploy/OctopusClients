using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability;

public class KubernetesMonitorResource
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string InstallationId { get; set; }

    [Required]
    public string MachineId { get; set; }
}