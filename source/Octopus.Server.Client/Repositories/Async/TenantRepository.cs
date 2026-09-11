using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ITenantRepository : IFindBySlug<TenantResource>, ICreate<TenantResource>, IModify<TenantResource>, IGet<TenantResource>, IDelete<TenantResource>, IFindByName<TenantResource>, IGetAll<TenantResource>, IFindByPartialName<TenantResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<MultiTenancyStatusResource> Status();
        Task<MultiTenancyStatusResource> Status(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task SetLogo(TenantResource tenant, string fileName, Stream contents);
        Task SetLogo(TenantResource tenant, string fileName, Stream contents, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TenantVariableResource> GetVariables(TenantResource tenant);
        Task<TenantVariableResource> GetVariables(TenantResource tenant, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TenantVariableResource> ModifyVariables(TenantResource tenant, TenantVariableResource variables);
        Task<TenantVariableResource> ModifyVariables(TenantResource tenant, TenantVariableResource variables, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TenantsMissingVariablesResource>> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null);
        Task<List<TenantsMissingVariablesResource>> GetMissingVariables(CancellationToken cancellationToken, string tenantId = null, string projectId = null, string environmentId = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TenantResource>> FindAll(string name, string[] tags = null, int pageSize = Int32.MaxValue);
        Task<List<TenantResource>> FindAll(string name, CancellationToken cancellationToken, string[] tags = null, int pageSize = Int32.MaxValue);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TenantEditor> CreateOrModify(string name);
        Task<TenantEditor> CreateOrModify(string name, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TenantEditor> CreateOrModify(string name, string description);
        Task<TenantEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TenantEditor> CreateOrModify(string name, string description, string cloneId);
        Task<TenantEditor> CreateOrModify(string name, string description, string cloneId, CancellationToken cancellationToken);
    }

    class TenantRepository : BasicRepository<TenantResource>, ITenantRepository
    {
        public TenantRepository(IOctopusAsyncRepository repository)
            : base(repository, "Tenants")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TenantVariableResource> GetVariables(TenantResource tenant)
            => GetVariables(tenant, CancellationToken.None);

        public Task<TenantVariableResource> GetVariables(TenantResource tenant, CancellationToken cancellationToken)
        {
            return Client.Get<TenantVariableResource>(tenant.Link("Variables"), cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tags"></param>
        /// <param name="pageSize">Number of items per page, setting to less than the total items still retreives all items, but uses multiple requests reducing memory load on the server</param>
        /// <returns></returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<TenantResource>> FindAll(string name, string[] tags, int pageSize = int.MaxValue)
            => FindAll(name, CancellationToken.None, tags, pageSize);

        public async Task<List<TenantResource>> FindAll(string name, CancellationToken cancellationToken, string[] tags = null, int pageSize = int.MaxValue)
        {
            return await Client.Get<List<TenantResource>>(await Repository.Link("Tenants").ConfigureAwait(false), new { id = IdValueConstant.IdAll, name, tags, take = pageSize }, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TenantVariableResource> ModifyVariables(TenantResource tenant, TenantVariableResource variables)
            => ModifyVariables(tenant, variables, CancellationToken.None);

        public Task<TenantVariableResource> ModifyVariables(TenantResource tenant, TenantVariableResource variables, CancellationToken cancellationToken)
        {
            return Client.Post<TenantVariableResource, TenantVariableResource>(tenant.Link("Variables"), variables, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<TenantsMissingVariablesResource>> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null)
            => GetMissingVariables(CancellationToken.None, tenantId, projectId, environmentId);

        public async Task<List<TenantsMissingVariablesResource>> GetMissingVariables(CancellationToken cancellationToken, string tenantId = null, string projectId = null, string environmentId = null)
        {
            return await Client.Get<List<TenantsMissingVariablesResource>>(await Repository.Link("TenantsMissingVariables").ConfigureAwait(false), new
            {
                tenantId = tenantId,
                projectId = projectId,
                environmentId = environmentId
            }, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MultiTenancyStatusResource> Status()
            => Status(CancellationToken.None);

        public async Task<MultiTenancyStatusResource> Status(CancellationToken cancellationToken)
        {
            return await Client.Get<MultiTenancyStatusResource>(await Repository.Link("TenantsStatus").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task SetLogo(TenantResource tenant, string fileName, Stream contents)
            => SetLogo(tenant, fileName, contents, CancellationToken.None);

        public Task SetLogo(TenantResource tenant, string fileName, Stream contents, CancellationToken cancellationToken)
        {
            return Client.Post(tenant.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TenantEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public Task<TenantEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            return new TenantEditor(this).CreateOrModify(name);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TenantEditor> CreateOrModify(string name, string description)
            => CreateOrModify(name, description, CancellationToken.None);

        public Task<TenantEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            return new TenantEditor(this).CreateOrModify(name, description);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TenantEditor> CreateOrModify(string name, string description, string cloneId)
            => CreateOrModify(name, description, cloneId, CancellationToken.None);

        public Task<TenantEditor> CreateOrModify(string name, string description, string cloneId, CancellationToken cancellationToken)
        {
            return new TenantEditor(this).CreateOrModify(name, description, cloneId);
        }
    }
}
