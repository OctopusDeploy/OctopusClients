using System;
using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace Octopus.Client.Model.Observability.ResourceEvents;

public class KubernetesEventResource
{
    [Required]
    public Instant FirstObservedTime { get; set; }

    [Required]
    public Instant LastObservedTime { get; set; }

    [Required]
    public int Count { get; set; }

    [Required]
    public string Action { get; set; }

    [Required]
    public string Reason { get; set; }

    [Required]
    public string Note { get; set; }

    [Required]
    public string ReportingController { get; set; }

    [Required]
    public string ReportingInstance { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string Manifest { get; set; }
}