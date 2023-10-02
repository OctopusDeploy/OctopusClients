using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ITenantRepository : ICreate<TenantResource>, IModify<TenantResource>, IGet<TenantResource>, IDelete<TenantResource>, IFindByName<TenantResource>, IGetAll<TenantResource>, IFindByPartialName<TenantResource>
    {
        Task<MultiTenancyStatusResource> Status();
        Task SetLogo(TenantResource tenant, string fileName, Stream contents);
        Task<TenantVariableResource> GetVariables(TenantResource tenant);
        Task<TenantVariableResource> ModifyVariables(TenantResource tenant, TenantVariableResource variables);
        Task<List<TenantsMissingVariablesResource>> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null);
        Task<List<TenantResource>> FindAll(string name, string[] tags = null, int pageSize = Int32.MaxValue);
        Task<TenantEditor> CreateOrModify(string name);
        Task<TenantEditor> CreateOrModify(string name, string description);
        Task<TenantEditor> CreateOrModify(string name, string description, string cloneId);
    }

    class TenantRepository : BasicRepository<TenantResource>, ITenantRepository
    {
        public TenantRepository(IOctopusAsyncRepository repository)
            : base(repository, "Tenants")
        {
        }

        public Task<TenantVariableResource> GetVariables(TenantResource tenant)
        {
            return Client.Get<TenantVariableResource>(tenant.Link("Variables"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tags"></param>
        /// <param name="pageSize">Number of items per page, setting to less than the total items still retrieves all items, but uses multiple requests reducing memory load on the server</param>
        /// <returns></returns>
        public async Task<List<TenantResource>> FindAll(string name, string[] tags, int pageSize = int.MaxValue)
        {
            return await Client.Get<List<TenantResource>>(await Repository.Link("Tenants").ConfigureAwait(false), new { id = IdValueConstant.IdAll, name, tags, take = pageSize }).ConfigureAwait(false);
        }

        public Task<TenantVariableResource> ModifyVariables(TenantResource tenant, TenantVariableResource variables)
        {
            return Client.Post<TenantVariableResource, TenantVariableResource>(tenant.Link("Variables"), variables);
        }

        public async Task<List<TenantsMissingVariablesResource>> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null)
        {
            return await Client.Get<List<TenantsMissingVariablesResource>>(await Repository.Link("TenantsMissingVariables").ConfigureAwait(false), new
            {
                tenantId = tenantId,
                projectId = projectId,
                environmentId = environmentId
            }).ConfigureAwait(false);
        }

        public async Task<MultiTenancyStatusResource> Status()
        {
            return await Client.Get<MultiTenancyStatusResource>(await Repository.Link("TenantsStatus").ConfigureAwait(false)).ConfigureAwait(false);
        }

        public Task SetLogo(TenantResource tenant, string fileName, Stream contents)
        {
            return Client.Post(tenant.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
        }

        public Task<TenantEditor> CreateOrModify(string name)
        {
            return new TenantEditor(this).CreateOrModify(name);
        }

        public Task<TenantEditor> CreateOrModify(string name, string description)
        {
            return new TenantEditor(this).CreateOrModify(name, description);
        }
        
        public Task<TenantEditor> CreateOrModify(string name, string description, string cloneId)
        {
            return new TenantEditor(this).CreateOrModify(name, description, cloneId);
        }
    }
}
