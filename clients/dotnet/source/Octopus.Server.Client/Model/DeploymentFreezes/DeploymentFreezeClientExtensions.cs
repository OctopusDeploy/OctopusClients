namespace Octopus.Client.Model.DeploymentFreezes;

public static class DeploymentFreezeClientExtensions
{
    public static CreateDeploymentFreezeResponse Create(this IOctopusClient client,
        CreateDeploymentFreezeCommand command)
    {
        return client.Create<CreateDeploymentFreezeCommand, CreateDeploymentFreezeResponse>("", command);
    }
}