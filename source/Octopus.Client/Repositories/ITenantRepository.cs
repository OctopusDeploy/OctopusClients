using System.Collections.Generic;
using System.IO;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITenantRepository : ICreate<TenantResource>, IModify<TenantResource>, IGet<TenantResource>, IDelete<TenantResource>, IFindByName<TenantResource>, IGetAll<TenantResource>
    {
        void SetLogo(TenantResource tenant, string fileName, Stream contents);
        TenantVariableResource GetVariables(TenantResource tenant);
        TenantVariableResource ModifyVariables(TenantResource tenant, TenantVariableResource variables);
        List<TenantsMissingVariablesResource> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null);
        List<TenantResource> FindAll(string name, string[] tags = null);
        TenantEditor CreateOrModify(string name);
    }
}