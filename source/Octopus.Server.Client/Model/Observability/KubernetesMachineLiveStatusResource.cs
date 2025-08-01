using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability;

public class KubernetesMachineLiveStatusResource
{
    [Required]
    public string MachineId { get; set; }

    [Required]
    public string Status { get; set; }

    [Required]
    public IReadOnlyCollection<KubernetesLiveStatusResource> Resources { get; set; }
}