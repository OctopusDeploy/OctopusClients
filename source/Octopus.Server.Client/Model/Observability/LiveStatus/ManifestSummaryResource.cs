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
    [Required]
    public IReadOnlyCollection<string> Containers { get; set; }
}
