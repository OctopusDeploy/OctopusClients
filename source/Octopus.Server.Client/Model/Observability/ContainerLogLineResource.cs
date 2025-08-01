using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace Octopus.Client.Model.Observability;

public class ContainerLogLineResource
{
    [Required]
    public Instant Timestamp { get; set; }

    [Required]
    public string Message { get; set; }
}