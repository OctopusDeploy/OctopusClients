using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability;

public class RegisterKubernetesMonitorResponse
{
    [Required]
    public KubernetesMonitorResource Resource { get; set; }

    [Required]
    public string AuthenticationToken { get; set; }

    [Required]
    public string CertificateThumbprint { get; set; }
}