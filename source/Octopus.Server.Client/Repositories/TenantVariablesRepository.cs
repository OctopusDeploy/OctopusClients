using System.Collections.Generic;
using Octopus.Client.Model;
using Octopus.Client.Model.TenantVariables;

namespace Octopus.Client.Repositories
{
    public interface ITenantVariablesRepository : IGetAll<TenantVariableResource>
    {
        List<TenantVariableResource> GetAll(ProjectResource projectResource);
        ModifyCommonVariablesByTenantIdResponse Modify(ModifyCommonVariablesByTenantIdCommand command);
    }


    class TenantVariablesRepository : BasicRepository<TenantVariableResource>, ITenantVariablesRepository
    {
        public ModifyCommonVariablesByTenantIdResponse Modify(ModifyCommonVariablesByTenantIdCommand command)
        {
            const string link = "/api/{spaceId}/tenants/{tenantId}/commonvariables";

            var response =
                Client.Update<ModifyCommonVariablesByTenantIdCommand, ModifyCommonVariablesByTenantIdResponse>(link,
                    command, pathParameters: new { command.SpaceId, command.TenantId });
            return response;
        }

        public ModifyProjectVariablesByTenantIdResponse Modify(ModifyProjectVariablesByTenantIdCommand command)
        {
            const string link = "/api/{spaceId}/tenants/{tenantId}/projectvariables";

            var response =
                Client.Update<ModifyProjectVariablesByTenantIdCommand, ModifyProjectVariablesByTenantIdResponse>(link,
                    command, pathParameters: new { command.SpaceId, command.TenantId });
            return response;
        }

        public List<TenantVariableResource> GetAll(ProjectResource projectResource)
        {
            return Client.Get<List<TenantVariableResource>>(Repository.Link("TenantVariables"), new
            {
                projectId = projectResource?.Id
            });
        }

        public TenantVariablesRepository(IOctopusRepository repository)
            : base(repository, "TenantVariables")
        {
        }
    }
}