using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.TenantVariables;

namespace Octopus.Client.Repositories.Async
{
    public interface ITenantVariablesRepository : IGetAll<TenantVariableResource>
    {
        Task<List<TenantVariableResource>> GetAll(ProjectResource projectResource);
        Task<ModifyCommonVariablesByTenantIdResponse> Modify(ModifyCommonVariablesByTenantIdCommand command, CancellationToken cancellationToken);
    }

    class TenantVariablesRepository : BasicRepository<TenantVariableResource>, ITenantVariablesRepository
    {
        public async Task<List<TenantVariableResource>> GetAll(ProjectResource projectResource)
        {
            return await Client.Get<List<TenantVariableResource>>(await Repository.Link("TenantVariables").ConfigureAwait(false), new
            {
                projectId = projectResource?.Id
            }).ConfigureAwait(false);
        }

        public async Task<ModifyCommonVariablesByTenantIdResponse> Modify(ModifyCommonVariablesByTenantIdCommand command, CancellationToken cancellationToken)
        {
            const string link = "/api/{spaceId}/tenants/{tenantId}/commonvariables";

            var response =
                await Client.Update<ModifyCommonVariablesByTenantIdCommand, ModifyCommonVariablesByTenantIdResponse>(link,
                    command, pathParameters: new { command.SpaceId, command.TenantId }, cancellationToken);
            return response;
        }

        public TenantVariablesRepository(IOctopusAsyncRepository repository) 
            : base(repository, "TenantVariables")
        {
        }
    }
}
