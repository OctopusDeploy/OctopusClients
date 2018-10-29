using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ITenantVariablesRepository : IGetAll<TenantVariableResource>
    {
        Task<List<TenantVariableResource>> GetAll(ProjectResource projectResource);
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

        public TenantVariablesRepository(IOctopusAsyncRepository repository) 
            : base(repository, "TenantVariables")
        {
        }
    }
}
