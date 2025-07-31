using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability;

public class GetContainerLogsRequest
{
    [Required]
    public string SessionId { get; set; }
}