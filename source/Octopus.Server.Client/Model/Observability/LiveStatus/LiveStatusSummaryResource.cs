using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.LiveStatus;

public class LiveStatusSummaryResource
{
    [Required]
    public string Status { get; set; }

    [Required]
    public DateTimeOffset LastUpdated { get; set; }

    [Required]
    public string SyncStatus { get; set; }
}
