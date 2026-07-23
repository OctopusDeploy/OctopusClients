namespace Octopus.Client.Model.RateLimiting;

public class RateLimitingPolicyResource(
    string id,
    bool isBuiltIn,
    string name,
    bool isEnabled,
    RateLimitingPolicyScopeType scopeType,
    int requestsPerHour,
    int burstLimit,
    bool auditMode)
{
    public string Id { get; } = id;
    public bool IsBuiltIn { get; } = isBuiltIn;
    public string Name { get; } = name;
    public bool IsEnabled { get; } = isEnabled;
    public RateLimitingPolicyScopeType ScopeType { get; } = scopeType;
    public int RequestsPerHour { get; } = requestsPerHour;
    public int BurstLimit { get; } = burstLimit;
    public bool AuditMode { get; } = auditMode;
}
