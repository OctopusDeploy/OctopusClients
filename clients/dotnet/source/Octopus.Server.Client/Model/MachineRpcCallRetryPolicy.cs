using System;

namespace Octopus.Client.Model;

public class MachineRpcCallRetryPolicy
{
    public bool Enabled { get; set; } = true;
    public TimeSpan RetryDuration { get; set; } = TimeSpan.FromMinutes(2.5);
    public TimeSpan HealthCheckRetryDuration { get; set; } = TimeSpan.FromMinutes(2.5);
}
