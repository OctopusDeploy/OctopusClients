using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability;

public class MonitorErrorResource
{
    [Required]
    public string ErrorMessage { get; set; }

    [Required]
    public string ErrorCode { get; set; }
}