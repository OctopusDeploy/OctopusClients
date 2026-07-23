using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.RateLimiting;

public class ModifyRateLimitingPolicyCommand
{
    [Required] public string Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public bool IsEnabled { get; set; }
    [Required] public RateLimitingPolicyScopeType ScopeType { get; set; }
    [Required] public int RequestsPerHour { get; set; }
    [Required] public int BurstLimit { get; set; }
    [Required] public bool AuditMode { get; set; }
}
