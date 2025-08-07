using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.KubernetesMonitor
{
    /// <summary>
    /// Get a Kubernetes Monitor by ID
    /// </summary>
    public class GetKubernetesMonitorByIdRequest
    {
        /// <summary>
        /// Id of the Kubernetes Monitor
        /// </summary>
        [Required]
        public string Id { get; set; } = null!;
    }
}
