using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.KubernetesMonitor;

public class RegisterKubernetesMonitorCommand
{
    /// <summary>
    /// Installation ID of that uniquely identifies the physical installation of the agent.
    /// </summary>
    [Required]
    public Guid InstallationId { get; set; }

    /// <summary>
    /// Machine ID of that uniquely identifies the deployment target or worker that is being observed
    /// </summary>
    [Required]
    public string MachineId { get; set; }
}
