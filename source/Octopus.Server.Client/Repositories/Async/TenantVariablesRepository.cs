using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.TenantVariables;

namespace Octopus.Client.Repositories.Async
{
    public interface ITenantVariablesRepository : IGetAll<TenantVariableResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TenantVariableResource>> GetAll(ProjectResource projectResource);
        Task<List<TenantVariableResource>> GetAll(ProjectResource projectResource, CancellationToken cancellationToken);

        Task<GetCommonVariablesByTenantIdResponse> Get(GetCommonVariablesByTenantIdRequest request,
            CancellationToken cancellationToken);

        Task<GetProjectVariablesByTenantIdResponse> Get(GetProjectVariablesByTenantIdRequest request,
            CancellationToken cancellationToken);

        Task<ModifyCommonVariablesByTenantIdResponse> Modify(ModifyCommonVariablesByTenantIdCommand command,
            CancellationToken cancellationToken);

        Task<ModifyProjectVariablesByTenantIdResponse> Modify(ModifyProjectVariablesByTenantIdCommand command,
            CancellationToken cancellationToken);
    }

    class TenantVariablesRepository : BasicRepository<TenantVariableResource>, ITenantVariablesRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<TenantVariableResource>> GetAll(ProjectResource projectResource)
            => GetAll(projectResource, CancellationToken.None);

        public async Task<List<TenantVariableResource>> GetAll(ProjectResource projectResource, CancellationToken cancellationToken)
        {
            return await Client.Get<List<TenantVariableResource>>(
                await Repository.Link("TenantVariables").ConfigureAwait(false), new
                {
                    projectId = projectResource?.Id
                }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<GetCommonVariablesByTenantIdResponse> Get(GetCommonVariablesByTenantIdRequest request,
            CancellationToken cancellationToken)
        {
            const string link = "/api/{spaceId}/tenants/{tenantId}/commonvariables?includeMissingVariables={includeMissingVariables}";

            var response =
                await Client.Get<GetCommonVariablesByTenantIdResponse>(link, new { request.SpaceId, request.TenantId, request.IncludeMissingVariables },
                    cancellationToken);
            return response;
        }

        public async Task<GetProjectVariablesByTenantIdResponse> Get(GetProjectVariablesByTenantIdRequest request,
            CancellationToken cancellationToken)
        {
            const string link = "/api/{spaceId}/tenants/{tenantId}/projectvariables?includeMissingVariables={includeMissingVariables}";

            var response =
                await Client.Get<GetProjectVariablesByTenantIdResponse>(link, new { request.SpaceId, request.TenantId, request.IncludeMissingVariables },
                    cancellationToken);
            return response;
        }

        public async Task<ModifyCommonVariablesByTenantIdResponse> Modify(
            ModifyCommonVariablesByTenantIdCommand command, CancellationToken cancellationToken)
        {
            const string link = "/api/{spaceId}/tenants/{tenantId}/commonvariables";

            var response =
                await Client.Update<ModifyCommonVariablesByTenantIdCommand, ModifyCommonVariablesByTenantIdResponse>(
                    link,
                    command, pathParameters: new { command.SpaceId, command.TenantId }, cancellationToken);
            return response;
        }

        public async Task<ModifyProjectVariablesByTenantIdResponse> Modify(
            ModifyProjectVariablesByTenantIdCommand command, CancellationToken cancellationToken)
        {
            const string link = "/api/{spaceId}/tenants/{tenantId}/projectvariables";

            var response =
                await Client.Update<ModifyProjectVariablesByTenantIdCommand, ModifyProjectVariablesByTenantIdResponse>(
                    link,
                    command, pathParameters: new { command.SpaceId, command.TenantId }, cancellationToken);
            return response;
        }

        public TenantVariablesRepository(IOctopusAsyncRepository repository)
            : base(repository, "TenantVariables")
        {
        }
    }
}
