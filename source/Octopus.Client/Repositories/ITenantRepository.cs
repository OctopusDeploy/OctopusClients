using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITenantRepository : ICreate<TenantResource>, IModify<TenantResource>, IGet<TenantResource>, IDelete<TenantResource>, IFindByName<TenantResource>, IGetAll<TenantResource>
    {
        Task SetLogo(TenantResource tenant, string fileName, Stream contents);
        Task<TenantVariableResource> GetVariables(TenantResource tenant);
        Task<TenantVariableResource> ModifyVariables(TenantResource tenant, TenantVariableResource variables);
        Task<List<TenantsMissingVariablesResource>> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null);
        Task<List<TenantResource>> FindAll(string name, string[] tags = null);
        Task<TenantEditor> CreateOrModify(string name);
    }
}