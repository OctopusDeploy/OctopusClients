namespace Octopus.Client.Model
{
    public enum CalamariUpdateBehavior
    {
        UpdateOnDeployment,
        UpdateOnNewMachine,
        UpdateAlways
    }

    public enum TentacleUpdateBehavior
    {
        NeverUpdate,
        Update
    }
    
    public enum KubernetesAgentUpdateBehavior
    {
        NeverUpdate,
        Update
    }

    public class MachineUpdatePolicy
    {
        public CalamariUpdateBehavior CalamariUpdateBehavior { get; set; }
        public TentacleUpdateBehavior TentacleUpdateBehavior { get; set; }
        public KubernetesAgentUpdateBehavior KubernetesAgentUpdateBehavior { get; set; }
        public string TentacleUpdateAccountId { get; set; }
    }
}