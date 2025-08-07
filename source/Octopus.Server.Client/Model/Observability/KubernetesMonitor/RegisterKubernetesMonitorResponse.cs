using System;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Model.Observability.LiveStatus;

namespace Octopus.Client.Model.Observability.KubernetesMonitor;

public class RegisterKubernetesMonitorResponse
{
    [Required]
    public KubernetesMonitorResource Resource { get; set; }

    [Required]
    public string AuthenticationToken { get; set; }

    [Required]
    public string CertificateThumbprint { get; set; }
}