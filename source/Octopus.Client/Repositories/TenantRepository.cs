using System;
using System.Collections.Generic;
using System.IO;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITenantRepository : ICreate<TenantResource>, IModify<TenantResource>, IGet<TenantResource>, IDelete<TenantResource>, IFindByName<TenantResource>, IGetAll<TenantResource>
    {
        MultiTenancyStatusResource Status();
        void SetLogo(TenantResource tenant, string fileName, Stream contents);
        TenantVariableResource GetVariables(TenantResource tenant);
        TenantVariableResource ModifyVariables(TenantResource tenant, TenantVariableResource variables);
        List<TenantsMissingVariablesResource> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null);
        List<TenantResource> FindAll(string name, string[] tags = null, int pageSize = Int32.MaxValue);
        TenantEditor CreateOrModify(string name);
    }
    
    class TenantRepository : BasicRepository<TenantResource>, ITenantRepository
    {
        public TenantRepository(IOctopusRepository repository)
            : base(repository, "Tenants")
        {
        }

        public TenantVariableResource GetVariables(TenantResource tenant)
        {
            return Client.Get<TenantVariableResource>(tenant.Link("Variables"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tags"></param>
        /// <param name="pageSize">Number of items per page, setting to less than the total items still retreives all items, but uses multiple requests reducing memory load on the server</param>
        /// <returns></returns>
        public List<TenantResource> FindAll(string name, string[] tags, int pageSize = Int32.MaxValue)
        {
            return Client.Get<List<TenantResource>>(Repository.Link("Tenants"), new { id = IdValueConstant.IdAll, name, tags, take = pageSize });
        }

        public TenantVariableResource ModifyVariables(TenantResource tenant, TenantVariableResource variables)
        {
            return Client.Post<TenantVariableResource, TenantVariableResource>(tenant.Link("Variables"), variables);
        }

        public List<TenantsMissingVariablesResource> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null)
        {
            return Client.Get<List<TenantsMissingVariablesResource>>(Repository.Link("TenantsMissingVariables"), new
            {
                tenantId = tenantId,
                projectId = projectId,
                environmentId = environmentId
            });
        }

        public MultiTenancyStatusResource Status()
        {
            return Client.Get<MultiTenancyStatusResource>(Repository.Link("TenantsStatus"));
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