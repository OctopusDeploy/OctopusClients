using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TenantVariablesEditor> Load()
            => Load(CancellationToken.None);

        public async Task<TenantVariablesEditor> Load(CancellationToken cancellationToken)
        {
            Instance = await repository.GetVariables(tenant, cancellationToken).ConfigureAwait(false);
            return this;
        }

        public TenantVariablesEditor Customize(Action<TenantVariableResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TenantVariablesEditor> Save()
            => Save(CancellationToken.None);

        public async Task<TenantVariablesEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.ModifyVariables(tenant, Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }

        public TenantVariablesEditor SetProjectVariableValue(
            ProjectResource projectResource,
            EnvironmentResource environmentResource,
            string templateName,
            PropertyValueResource value)
        {
            Instance.SetProjectVariableValue(projectResource, environmentResource, templateName, value);
            return this;
        }

        public TenantVariablesEditor SetLibraryVariableValue(
            LibraryVariableSetResource libraryVariableSetResource,
            string templateName,
            PropertyValueResource value)
        {
            Instance.SetLibraryVariableValue(libraryVariableSetResource, templateName, value);
            return this;
        }
    }
}
