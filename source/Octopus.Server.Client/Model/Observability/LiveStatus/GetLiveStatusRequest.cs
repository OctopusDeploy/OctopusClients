using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.LiveStatus;

public class GetLiveStatusRequest
{
    [Required]
    public string ProjectId { get; set; }

    [Required]
    public string EnvironmentId { get; set; }

    public string TenantId { get; set; }

    public bool SummaryOnly { get; set; }
}
