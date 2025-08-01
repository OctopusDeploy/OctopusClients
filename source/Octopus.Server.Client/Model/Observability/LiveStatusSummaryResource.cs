using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace Octopus.Client.Model.Observability;

public class LiveStatusSummaryResource
{
    [Required]
    public string Status { get; set; }

    [Required]
    public Instant LastUpdated { get; set; }
}