using System;
using System.IO;
using System.Threading;
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
            variables = new Lazy<Task<TenantVariablesEditor>>(() => new TenantVariablesEditor(repository, Instance).Load(CancellationToken.None));
        }

        public TenantResource Instance { get; private set; }

        public Task<TenantVariablesEditor> Variables => variables.Value;

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TenantEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public async Task<TenantEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new TenantResource
                {
                    Name = name,
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TenantEditor> CreateOrModify(string name, string description, string cloneId = null)
            => CreateOrModify(name, description, cloneId, CancellationToken.None);

        public Task<TenantEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
            => CreateOrModify(name, description, null, cancellationToken);

        public async Task<TenantEditor> CreateOrModify(string name, string description, string cloneId, CancellationToken cancellationToken)
        {
            var baseRepository = ((TenantRepository)repository).Repository;
            if (!await baseRepository.HasLinkParameter("Tenants", "clone"))
                throw new OperationNotSupportedByOctopusServerException(cloneId == null
                    ? "Tenant Descriptions requires Octopus version 2019.8.0 or newer."
                    : "Cloning Tenants requires Octopus version 2019.8.0 or newer.", "2019.8.0");

            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new TenantResource
                {
                    Name = name,
                    Description = description,
                }, new { clone = cloneId }, cancellationToken
                ).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;
                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TenantEditor> SetLogo(string logoFilePath)
            => SetLogo(logoFilePath, CancellationToken.None);

        public async Task<TenantEditor> SetLogo(string logoFilePath, CancellationToken cancellationToken)
        {
            using (var stream = new FileStream(logoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await repository.SetLogo(Instance, Path.GetFileName(logoFilePath), stream, cancellationToken).ConfigureAwait(false);
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TenantEditor> Save()
            => Save(CancellationToken.None);

        public async Task<TenantEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            if (variables.IsValueCreated)
            {
                var vars = await variables.Value.ConfigureAwait(false);
                await vars.Save(cancellationToken).ConfigureAwait(false);
            }
            return this;
        }
    }
}
