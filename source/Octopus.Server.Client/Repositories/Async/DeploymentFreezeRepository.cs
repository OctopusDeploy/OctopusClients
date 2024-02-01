using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.DeploymentFreezes;

namespace Octopus.Client.Repositories.Async;

public interface IDeploymentFreezeRepository
{
    Task<CreateDeploymentFreezeResponse> Create(CreateDeploymentFreezeCommand command, CancellationToken cancellationToken);
}

public class DeploymentFreezeRepository(IOctopusAsyncClient client) : IDeploymentFreezeRepository
{
    public async Task<CreateDeploymentFreezeResponse> Create(CreateDeploymentFreezeCommand command, CancellationToken cancellationToken)
    {
        var link = await client.Repository.Link("DeploymentFreezes");

        var response = await client.Create<CreateDeploymentFreezeCommand, CreateDeploymentFreezeResponse>(link, command, null, cancellationToken);
        return response;
    }
}