
namespace Octopus.Client.Model
{
    public enum MachineScriptPolicyRunType : int
    {
        InheritFromDefault = 0,
        Inline,
        OnlyConnectivity
    }

    public class MachineScriptPolicy
    {
        public MachineScriptPolicyRunType RunType { get; set; }
        public string ScriptBody { get; set; }
    }
}
