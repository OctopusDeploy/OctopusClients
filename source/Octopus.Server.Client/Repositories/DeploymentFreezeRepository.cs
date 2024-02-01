using Octopus.Client.Model.DeploymentFreezes;

namespace Octopus.Client.Repositories;

public interface IDeploymentFreezeRepository
{
    CreateDeploymentFreezeResponse Create(CreateDeploymentFreezeCommand command);
}

public class DeploymentFreezeRepository(IOctopusClient client) : IDeploymentFreezeRepository
{
    public CreateDeploymentFreezeResponse Create(CreateDeploymentFreezeCommand command)
    {
        var link = client.Repository.Link("DeploymentFreezes");

        return client.Create<CreateDeploymentFreezeCommand, CreateDeploymentFreezeResponse>(link, command);
    }
}