using System;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
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
        
        public async Task<TenantEditor> CreateOrModify(string name, string description, string cloneId = null)
        {
            var baseRepository = ((TenantRepository) repository).Repository;
            if (!await baseRepository.HasLinkParameter("Tenants", "clone"))
                throw new OperationNotSupportedByOctopusServerException(cloneId == null
                    ? "Tenant Descriptions requires Octopus version 2019.8.0 or newer."
                    : "Cloning Tenants requires Octopus version 2019.8.0 or newer.", "2019.8.0");

            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new TenantResource
                    {
                        Name = name,
                        Description = description,
                    }, new { clone = cloneId }
                ).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;
                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public async Task<TenantEditor> SetLogo(string logoFilePath)
        {
            using (var stream = new FileStream(logoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await repository.SetLogo(Instance, Path.GetFileName(logoFilePath), stream).ConfigureAwait(false);
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