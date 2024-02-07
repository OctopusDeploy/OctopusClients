using Octopus.Client.Model.DeploymentFreezes;

namespace Octopus.Client.Repositories;

public interface IDeploymentFreezeRepository
{
    GetDeploymentFreezeByIdResponse Get(GetDeploymentFreezeByIdRequest request);
    CreateDeploymentFreezeResponse Create(CreateDeploymentFreezeCommand command);

    ModifyDeploymentFreezeResponse Modify(ModifyDeploymentFreezeCommand command);

    DeleteDeploymentFreezeResponse Delete(DeleteDeploymentFreezeCommand command);
}

public class DeploymentFreezeRepository(IOctopusClient client) : IDeploymentFreezeRepository
{
    public GetDeploymentFreezeByIdResponse Get(GetDeploymentFreezeByIdRequest request)
    {
        var link = client.Repository.Link("DeploymentFreezes");

        return client.Get<GetDeploymentFreezeByIdRequest, GetDeploymentFreezeByIdResponse>(link, request);
    }

    public CreateDeploymentFreezeResponse Create(CreateDeploymentFreezeCommand command)
    {
        var link = client.Repository.Link("DeploymentFreezes");

        return client.Create<CreateDeploymentFreezeCommand, CreateDeploymentFreezeResponse>(link, command);
    }


    public ModifyDeploymentFreezeResponse Modify(ModifyDeploymentFreezeCommand command)
    {
        var link = client.Repository.Link("DeploymentFreezes");

        return client.Update<ModifyDeploymentFreezeCommand, ModifyDeploymentFreezeResponse>(link, command);
    }

    public DeleteDeploymentFreezeResponse Delete(DeleteDeploymentFreezeCommand command)
    {
        var link = client.Repository.Link("DeploymentFreezes");

        return client.Delete<DeleteDeploymentFreezeCommand, DeleteDeploymentFreezeResponse>(link, command);
    }
}