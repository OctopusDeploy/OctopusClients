using System;

namespace Octopus.Client.Model.RateLimiting;

internal static class RateLimitingPolicyRepositoryPaths
{
    public const string List = "~/api/ratelimitingpolicies?skip={skip}&take={take}";
    public const string GetById = "~/api/ratelimitingpolicies/{id}";
    public const string Modify = "~/api/ratelimitingpolicies/{id}";
}
