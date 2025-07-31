using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability;

public class KubernetesLiveStatusManifestDetailsResource
{
    [Required]
    public Dictionary<string, string> Labels { get; set; }

    [Required]
    public Dictionary<string, string> Annotations { get; set; }

    [Required]
    public DateTimeOffset CreationTimestamp { get; set; }

    [Required]
    public IReadOnlyCollection<string> Containers { get; set; }
}