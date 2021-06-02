
using System;

namespace Octopus.Client.Model
{
    public enum MachineScriptPolicyRunType : int
    {
        InheritFromDefault = 0,
        Inline,
        [Obsolete("The connectivity setting is now configured per " + nameof(MachineHealthCheckPolicy) + " using the property " + nameof(MachineHealthCheckPolicy.HealthCheckType), true)]
        OnlyConnectivity
    }

    public class MachineScriptPolicy
    {
        public MachineScriptPolicyRunType RunType { get; set; }
        public string ScriptBody { get; set; }
    }
}
