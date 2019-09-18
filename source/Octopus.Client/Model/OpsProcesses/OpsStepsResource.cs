namespace Octopus.Client.Model.OpsProcesses
{
    public class OpsStepsResource : DeploymentProcessBaseResource
    {
        public string ProjectId { get; set; }

        public string OpsProcessId
        {
            get => base.OwnerId;
            set => base.OwnerId = value;
        }
    }
}