using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.ContainerLogs;

public class GetContainerLogsResponse
{
    [Required]
    public ICollection<ContainerLogLineResource> Logs { get; set; }

    [Required]
    public bool IsSessionCompleted { get; set; }

    public MonitorErrorResource Error { get; set; }
}