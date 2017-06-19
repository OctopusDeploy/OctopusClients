using System;
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
    
    class TenantRepository : BasicRepository<TenantResource>, ITenantRepository
    {
        public TenantRepository(IOctopusClient client)
            : base(client, "Tenants")
        {
        }

        public TenantVariableResource GetVariables(TenantResource tenant)
        {
            return Client.Get<TenantVariableResource>(tenant.Link("Variables"));
        }

        public List<TenantResource> FindAll(string name, string[] tags)
        {
            return Client.Get<List<TenantResource>>(Client.RootDocument.Link("Tenants"), new { id = "all", name, tags, take = int.MaxValue });
        }

        public TenantVariableResource ModifyVariables(TenantResource tenant, TenantVariableResource variables)
        {
            return Client.Post<TenantVariableResource, TenantVariableResource>(tenant.Link("Variables"), variables);
        }

        public List<TenantsMissingVariablesResource> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null)
        {
            return Client.Get<List<TenantsMissingVariablesResource>>(Client.RootDocument.Link("TenantsMissingVariables"), new
            {
                tenantId = tenantId,
                projectId = projectId,
                environmentId = environmentId
            });
        }

        public void SetLogo(TenantResource tenant, string fileName, Stream contents)
        {
            Client.Post(tenant.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
        }

        public TenantEditor CreateOrModify(string name)
        {
            return new TenantEditor(this).CreateOrModify(name);
        }
    }
}