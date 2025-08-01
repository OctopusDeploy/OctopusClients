using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability;

public class GetResourceEventsRequest
{
    [Required]
    public string SessionId { get; set; }
}