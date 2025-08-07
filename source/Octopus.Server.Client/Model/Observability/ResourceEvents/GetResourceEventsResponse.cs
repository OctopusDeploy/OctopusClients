using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.ResourceEvents;

public class GetResourceEventsResponse
{
    [Required]
    public ICollection<KubernetesEventResource> Events { get; set; }

    [Required]
    public bool IsSessionCompleted { get; set; }

    public MonitorErrorResource Error { get; set; }
}