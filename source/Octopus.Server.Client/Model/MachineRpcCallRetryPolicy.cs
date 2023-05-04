using System;

namespace Octopus.Client.Model
{
    public class MachineRpcCallRetryPolicy
    {
        public bool Enabled { get; set; } = false;

        public TimeSpan RetryDuration { get; set; } = TimeSpan.FromMinutes(10);

        public TimeSpan HealthyMachineHealthCheckRetryDuration { get; set; } = TimeSpan.FromMinutes(10);

        public TimeSpan UnHealthyMachineHealthCheckRetryDuration { get; set; } = TimeSpan.FromMinutes(2);
    }
}
