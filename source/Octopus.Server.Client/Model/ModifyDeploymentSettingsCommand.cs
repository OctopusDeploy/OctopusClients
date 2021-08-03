namespace Octopus.Client.Model
{
    public class ModifyDeploymentSettingsCommand : DeploymentSettingsResource, ICommitCommand
    {
        public string ChangeDescription { get; set; }
    }
}