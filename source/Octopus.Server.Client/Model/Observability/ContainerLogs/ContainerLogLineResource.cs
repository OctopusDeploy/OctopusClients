using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.ContainerLogs;

public class ContainerLogLineResource
{
    [Required]
    public DateTimeOffset Timestamp { get; set; }

    [Required]
    public string Message { get; set; }
}
