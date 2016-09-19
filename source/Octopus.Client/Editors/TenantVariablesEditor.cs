using System;
using System.Threading.Tasks;
using Octopus.Client.Editors.DeploymentProcess;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class TenantVariablesEditor : IResourceEditor<TenantVariableResource, TenantVariablesEditor>
    {
        private readonly ITenantRepository repository;
        private readonly TenantResource tenant;

        public TenantVariablesEditor(ITenantRepository repository, TenantResource tenant)
        {
            this.repository = repository;
            this.tenant = tenant;
        }

        public TenantVariableResource Instance { get; private set; }

        public async Task<TenantVariablesEditor> Load()
        {
            Instance = await repository.GetVariables(tenant).ConfigureAwait(false);
            return this;
        }

        public TenantVariablesEditor Customize(Action<TenantVariableResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<TenantVariablesEditor> Save()
        {
            Instance = await repository.ModifyVariables(tenant, Instance).ConfigureAwait(false);
            return this;
        }
    }
}