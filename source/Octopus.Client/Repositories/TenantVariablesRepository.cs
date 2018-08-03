using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITenantVariablesRepository : IGetAll<TenantVariableResource>
    {
        List<TenantVariableResource> GetAll(ProjectResource projectResource);
    }

    class TenantVariablesRepository : BasicRepository<TenantVariableResource>, ITenantVariablesRepository
    {
        public List<TenantVariableResource> GetAll(ProjectResource projectResource)
        {
            return Client.Get<List<TenantVariableResource>>(Client.Link("TenantVariables"), new
            {
                projectId = projectResource?.Id
            });
        }

        public TenantVariablesRepository(IOctopusClient client) 
            : base(client, "TenantVariables")
        {
        }
    }
}
