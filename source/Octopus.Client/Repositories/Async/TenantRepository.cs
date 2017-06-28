using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ITenantRepository : ICreate<TenantResource>, IModify<TenantResource>, IGet<TenantResource>, IDelete<TenantResource>, IFindByName<TenantResource>, IGetAll<TenantResource>
    {
        Task SetLogo(TenantResource tenant, string fileName, Stream contents);
        Task<TenantVariableResource> GetVariables(TenantResource tenant);
        Task<TenantVariableResource> ModifyVariables(TenantResource tenant, TenantVariableResource variables);
        Task<List<TenantsMissingVariablesResource>> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null);
        Task<List<TenantResource>> FindAll(string name, string[] tags = null, int pageSize = Int32.MaxValue);
        Task<TenantEditor> CreateOrModify(string name);
    }

    class TenantRepository : BasicRepository<TenantResource>, ITenantRepository
    {
        public TenantRepository(IOctopusAsyncClient client)
            : base(client, "Tenants")
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
        /// <param name="pageSize">Number of items per page, setting to less than the total items still retreives all items, but uses multiple requests reducing memory load on the server</param>
        /// <returns></returns>
        public Task<List<TenantResource>> FindAll(string name, string[] tags, int pageSize = int.MaxValue)
        {
            return Client.Get<List<TenantResource>>(Client.RootDocument.Link("Tenants"), new { id = "all", name, tags, take = pageSize });
        }

        public Task<TenantVariableResource> ModifyVariables(TenantResource tenant, TenantVariableResource variables)
        {
            return Client.Post<TenantVariableResource, TenantVariableResource>(tenant.Link("Variables"), variables);
        }

        public Task<List<TenantsMissingVariablesResource>> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null)
        {
            return Client.Get<List<TenantsMissingVariablesResource>>(Client.RootDocument.Link("TenantsMissingVariables"), new
            {
                tenantId = tenantId,
                projectId = projectId,
                environmentId = environmentId
            });
        }

        public Task SetLogo(TenantResource tenant, string fileName, Stream contents)
        {
            return Client.Post(tenant.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
        }

        public Task<TenantEditor> CreateOrModify(string name)
        {
            return new TenantEditor(this).CreateOrModify(name);
        }
    }
}
