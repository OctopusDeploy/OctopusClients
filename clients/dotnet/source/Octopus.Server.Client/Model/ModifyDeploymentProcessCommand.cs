namespace Octopus.Client.Model
{
    public class ModifyDeploymentProcessCommand : DeploymentProcessResource, ICommitCommand
    {
        public string ChangeDescription { get; set; }
    }
}