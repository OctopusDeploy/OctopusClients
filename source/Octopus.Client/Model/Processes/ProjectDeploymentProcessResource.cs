namespace Octopus.Client.Model.Processes
{
    public class ProjectDeploymentProcessResource : ProcessResource
    {
        public override ProcessType Type => ProcessType.Deployment;
    }
}