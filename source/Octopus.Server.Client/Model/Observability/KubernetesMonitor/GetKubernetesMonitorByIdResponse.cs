using System;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Model.Observability.LiveStatus;

namespace Octopus.Client.Model.Observability.KubernetesMonitor
{
    /// <summary>
    /// The requested Kubernetes Monitor
    /// </summary>
    public class GetKubernetesMonitorByIdResponse
    {
        [Required]
        public KubernetesMonitorResource Resource { get; set; }
    }
}
