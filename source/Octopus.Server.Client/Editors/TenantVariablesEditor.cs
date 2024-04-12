using System;
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

        public TenantVariablesEditor Load()
        {
            Instance = repository.GetVariables(tenant);
            return this;
        }

        public TenantVariablesEditor Customize(Action<TenantVariableResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public TenantVariablesEditor Save()
        {
            Instance = repository.ModifyVariables(tenant, Instance);
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