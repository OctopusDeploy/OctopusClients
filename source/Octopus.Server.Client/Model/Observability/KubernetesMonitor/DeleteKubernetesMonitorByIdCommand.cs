using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.KubernetesMonitor
{
    /// <summary>
    /// Delete a Kubernetes Monitor by ID
    /// </summary>
    public class DeleteKubernetesMonitorByIdCommand
    {
        /// <summary>
        /// Id of the Kubernetes Monitor
        /// </summary>
        [Required]
        public string Id { get; set; }
    }
}
