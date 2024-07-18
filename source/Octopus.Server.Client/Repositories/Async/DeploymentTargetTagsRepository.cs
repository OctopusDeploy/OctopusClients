using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.DeploymentTargetTags;

namespace Octopus.Client.Repositories.Async;

public interface IDeploymentTargetTagsRepository
{
    Task<CreateDeploymentTargetTagResponse> Create(CreateDeploymentTargetTagCommand command,
        CancellationToken cancellationToken);
    
    Task<DeleteDeploymentTargetTagResponse> Delete(DeleteDeploymentTargetTagCommand command,
        CancellationToken cancellationToken);

    Task<GetDeploymentTargetTagByTagResponse> Get(GetDeploymentTargetTagByTagResponse request,
        CancellationToken cancellationToken);
    
    Task<GetDeploymentTargetTagsResponse> Get(GetDeploymentTargetTagsRequest request,
        CancellationToken cancellationToken);
}

public class DeploymentTargetTagsRepository(IOctopusAsyncClient client) : IDeploymentTargetTagsRepository
{
    public async Task<CreateDeploymentTargetTagResponse> Create(CreateDeploymentTargetTagCommand command,
        CancellationToken cancellationToken)
    {
        var link = await client.Repository.Link("DeploymentTargetTags");
        
        return await client.Create<CreateDeploymentTargetTagCommand, CreateDeploymentTargetTagResponse>(link, command, null, cancellationToken);
    }
    
    public async Task<DeleteDeploymentTargetTagResponse> Delete(DeleteDeploymentTargetTagCommand command,
        CancellationToken cancellationToken)
    {
        var link = await client.Repository.Link("DeploymentTargetTags");
        
        return await client.Delete<DeleteDeploymentTargetTagCommand, DeleteDeploymentTargetTagResponse>(link, command, cancellationToken);
    }
    
    public async Task<GetDeploymentTargetTagByTagResponse> Get(GetDeploymentTargetTagByTagResponse request,
        CancellationToken cancellationToken)
    {
        var link = await client.Repository.Link("DeploymentTargetTags");
        
        return await client.Get<GetDeploymentTargetTagByTagResponse>(link, request, cancellationToken);
    }
    
    public async Task<GetDeploymentTargetTagsResponse> Get(GetDeploymentTargetTagsRequest request,
        CancellationToken cancellationToken)
    {
        var link = await client.Repository.Link("DeploymentTargetTags");
        
        return await client.Get<GetDeploymentTargetTagsResponse>(link, request, cancellationToken);
    }
}