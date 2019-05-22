using System;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class MachineHealthCheckPolicy
    {
        [Obsolete("Use " + nameof(PowerShellHealthCheckPolicy) + " instead.")]
        public MachineScriptPolicy TentacleEndpointHealthCheckPolicy => PowerShellHealthCheckPolicy;
        public MachineScriptPolicy PowerShellHealthCheckPolicy { get; set; }
        [Obsolete("Use " + nameof(BashHealthCheckPolicy) + " instead.")]
        public MachineScriptPolicy SshEndpointHealthCheckPolicy => BashHealthCheckPolicy;
        public MachineScriptPolicy BashHealthCheckPolicy { get; set; }
        public TimeSpan HealthCheckInterval { get; set; }

        public HealthCheckType HealthCheckType { get; set; }

        public MachineHealthCheckPolicy()
        {
            PowerShellHealthCheckPolicy = new MachineScriptPolicy();
            BashHealthCheckPolicy = new MachineScriptPolicy();
            HealthCheckInterval = TimeSpan.FromHours(1);
            HealthCheckType = HealthCheckType.RunScript;
        }

        [JsonConstructor]
        public MachineHealthCheckPolicy(MachineScriptPolicy powerShellHealthCheckPolicy, MachineScriptPolicy bashHealthCheckPolicy)
        {
            PowerShellHealthCheckPolicy = powerShellHealthCheckPolicy;
            BashHealthCheckPolicy = bashHealthCheckPolicy;
        }
    }

    public enum HealthCheckType
    {
        RunScript,
        OnlyConnectivity
    }
}