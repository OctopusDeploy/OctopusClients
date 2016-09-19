using System;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Editors.DeploymentProcess;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class TenantEditor : IResourceEditor<TenantResource, TenantEditor>
    {
        private readonly ITenantRepository repository;
        private readonly Lazy<Task<TenantVariablesEditor>> variables;

        public TenantEditor(ITenantRepository repository)
        {
            this.repository = repository;
            variables = new Lazy<Task<TenantVariablesEditor>>(() => new TenantVariablesEditor(repository, Instance).Load());
        }

        public TenantResource Instance { get; private set; }

        public Task<TenantVariablesEditor> Variables => variables.Value;

        public async Task<TenantEditor> CreateOrModify(string name)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new TenantResource
                {
                    Name = name,
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public TenantEditor SetLogo(string logoFilePath)
        {
            using (var stream = new FileStream(logoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                repository.SetLogo(Instance, Path.GetFileName(logoFilePath), stream);
            }

            return this;
        }

        public TenantEditor ClearTags()
        {
            Instance.ClearTags();
            return this;
        }

        public TenantEditor WithTag(TagResource tag)
        {
            Instance.WithTag(tag);
            return this;
        }

        public TenantEditor ClearProjects()
        {
            Instance.ClearProjects();
            return this;
        }

        public TenantEditor ConnectToProjectAndEnvironments(ProjectResource project, params EnvironmentResource[] environments)
        {
            Instance.ConnectToProjectAndEnvironments(project, environments);
            return this;
        }

        public TenantEditor Customize(Action<TenantResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<TenantEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            if (variables.IsValueCreated)
            {
                var vars = await variables.Value.ConfigureAwait(false);
                await vars.Save().ConfigureAwait(false);
            }
            return this;
        }
    }
}