using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.DeploymentFreezes;

namespace Octopus.Client.Repositories.Async;

public interface IDeploymentFreezeRepository
{
    Task<GetDeploymentFreezeByIdResponse> Get(GetDeploymentFreezeByIdRequest request,
        CancellationToken cancellationToken);

    Task<GetDeploymentFreezesResponse> Get(GetDeploymentFreezesRequest request,
        CancellationToken cancellationToken);

    Task<CreateDeploymentFreezeResponse> Create(CreateDeploymentFreezeCommand command,
        CancellationToken cancellationToken);

    Task<ModifyDeploymentFreezeResponse> Modify(ModifyDeploymentFreezeCommand command,
        CancellationToken cancellationToken);

    Task<DeleteDeploymentFreezeResponse> Delete(DeleteDeploymentFreezeCommand command,
        CancellationToken cancellationToken);
}

public class DeploymentFreezeRepository(IOctopusAsyncClient client) : IDeploymentFreezeRepository
{
    public async Task<GetDeploymentFreezeByIdResponse> Get(GetDeploymentFreezeByIdRequest request,
        CancellationToken cancellationToken)
    {
        var link = await client.Repository.Link("DeploymentFreezes");

        var response = await client.Get<GetDeploymentFreezeByIdResponse>(link, request, cancellationToken);
        return response;
    }

    public async Task<GetDeploymentFreezesResponse> Get(GetDeploymentFreezesRequest request,
        CancellationToken cancellationToken)
    {
        var link = await client.Repository.Link("DeploymentFreezes");

        var response = await client.Get<GetDeploymentFreezesResponse>(link, request, cancellationToken);
        return response;
    }

    public async Task<CreateDeploymentFreezeResponse> Create(CreateDeploymentFreezeCommand command,
        CancellationToken cancellationToken)
    {
        var link = await client.Repository.Link("DeploymentFreezes");

        var response =  await client.Create<CreateDeploymentFreezeCommand, CreateDeploymentFreezeResponse>(link, command, null, cancellationToken);
        return response;
    }

    public async Task<ModifyDeploymentFreezeResponse> Modify(ModifyDeploymentFreezeCommand command,
        CancellationToken cancellationToken)
    {
        var link = await client.Repository.Link("DeploymentFreezes");
        var response = await client.Update<ModifyDeploymentFreezeCommand, ModifyDeploymentFreezeResponse>(link, command, new { id = command.Id }, cancellationToken);
        return response;
    }

    public async Task<DeleteDeploymentFreezeResponse> Delete(DeleteDeploymentFreezeCommand command,
        CancellationToken cancellationToken)
    {
        var link = await client.Repository.Link("DeploymentFreezes");
        var pathWithId = $"{link}/{command.Id}";
        return await client.Delete<DeleteDeploymentFreezeCommand, DeleteDeploymentFreezeResponse>(pathWithId, command, cancellationToken);
    }
}
