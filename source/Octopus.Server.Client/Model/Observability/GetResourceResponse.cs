using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability;

public class GetResourceResponse
{
    [Required]
    public KubernetesLiveStatusDetailedResource Resource { get; set; }
}