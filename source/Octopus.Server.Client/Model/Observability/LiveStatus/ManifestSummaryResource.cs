using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.LiveStatus;

public class ManifestSummaryResource
{
    [Required]
    public string Kind { get; set; }

    [Required]
    public Dictionary<string, string> Labels { get; set; }

    [Required]
    public Dictionary<string, string> Annotations { get; set; }

    [Required]
    public DateTimeOffset CreationTimestamp { get; set; }
}

public class PodManifestSummaryResource : ManifestSummaryResource
{
    [Obsolete("Superseded by ContainerDetails. Retained for backwards compatibility.")]
    [Required]
    public IReadOnlyCollection<string> Containers { get; set; }

    [Required]
    public IReadOnlyCollection<PodContainerResource> ContainerDetails { get; set; }
}

public class PodContainerResource
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Image { get; set; }
}
