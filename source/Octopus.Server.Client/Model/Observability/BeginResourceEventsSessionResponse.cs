using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability;

public class BeginResourceEventsSessionResponse
{
    [Required]
    public string SessionId { get; set; }
}