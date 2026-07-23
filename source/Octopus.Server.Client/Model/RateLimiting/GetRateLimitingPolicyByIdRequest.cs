using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.RateLimiting;

public class GetRateLimitingPolicyByIdRequest
{
    [Required] public string Id { get; set; }
}
