namespace Octopus.Client.Model
{
    public enum SkipMachineBehavior
    {
        None = 0,
        SkipUnavailableMachines
    }

    public class DeploymentConnectivityPolicy
    {
        public SkipMachineBehavior SkipMachineBehavior { get; set; }
        public ReferenceCollection TargetRoles { get; set; }

        public bool AllowDeploymentsToNoTargets { get; set; }

        public bool ExcludeUnhealthyTargets { get; set; }

        public DeploymentConnectivityPolicy()
        {
            TargetRoles = new ReferenceCollection();
        }
    }
}
