namespace Octopus.Client.Model.RateLimiting;

public enum RateLimitingPolicyScopeType
{
    Unauthenticated,
    AuthenticatedHuman,
    AuthenticatedAgent
}
