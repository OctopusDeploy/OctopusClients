using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.LiveStatus;

public class GetResourceResponse
{
    [Required]
    public KubernetesLiveStatusDetailedResource Resource { get; set; }
}