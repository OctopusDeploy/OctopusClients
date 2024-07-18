using Octopus.Client.Model.DeploymentTargetTags;

namespace Octopus.Client.Repositories;

public interface IDeploymentTargetTagsRepository
{
    CreateDeploymentTargetTagResponse Create(CreateDeploymentTargetTagCommand command);
    
    DeleteDeploymentTargetTagResponse Delete(DeleteDeploymentTargetTagCommand command);

    GetDeploymentTargetTagByTagResponse Get(GetDeploymentTargetTagByTagResponse request);
    
    GetDeploymentTargetTagsResponse Get(GetDeploymentTargetTagsRequest request);
}

public class DeploymentTargetTagsRepository(IOctopusClient client) : IDeploymentTargetTagsRepository
{
    public CreateDeploymentTargetTagResponse Create(CreateDeploymentTargetTagCommand command)
    {
        var link = client.Repository.Link("DeploymentTargetTags");
        
        return client.Create<CreateDeploymentTargetTagCommand, CreateDeploymentTargetTagResponse>(link, command);
    }
    
    public DeleteDeploymentTargetTagResponse Delete(DeleteDeploymentTargetTagCommand command)
    {
        var link = client.Repository.Link("DeploymentTargetTags");
        
        return client.Delete<DeleteDeploymentTargetTagCommand, DeleteDeploymentTargetTagResponse>(link, command);
    }
    
    public GetDeploymentTargetTagByTagResponse Get(GetDeploymentTargetTagByTagResponse request)
    {
        var link = client.Repository.Link("DeploymentTargetTags");
        
        return client.Get<GetDeploymentTargetTagByTagResponse>(link, request);
    }
    
    public GetDeploymentTargetTagsResponse Get(GetDeploymentTargetTagsRequest request)
    {
        var link = client.Repository.Link("DeploymentTargetTags");
        
        return client.Get<GetDeploymentTargetTagsResponse>(link, request);
    }
}