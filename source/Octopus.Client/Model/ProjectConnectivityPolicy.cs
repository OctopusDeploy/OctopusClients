namespace Octopus.Client.Model
{
    public enum SkipMachineBehavior
    {
        None = 0,
        SkipUnavailableMachines
    }

    public class ProjectConnectivityPolicy
    {
        public SkipMachineBehavior SkipMachineBehavior { get; set; }
        public ReferenceCollection TargetRoles { get; set; }

        public bool AllowDeploymentsToEmptyEnvironments { get; set; }

        public ProjectConnectivityPolicy()
        {
            TargetRoles = new ReferenceCollection();
        }
    }
}
