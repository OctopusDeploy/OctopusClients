using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.ContainerLogs;

public class GetContainerLogsRequest
{
    [Required]
    public string SessionId { get; set; }
}