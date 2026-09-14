using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class EnvironmentEditor : IResourceEditor<EnvironmentResource, EnvironmentEditor>
    {
        private readonly IEnvironmentRepository repository;

        public EnvironmentEditor(IEnvironmentRepository repository)
        {
            this.repository = repository;
        }

        public EnvironmentResource Instance { get; private set; }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<EnvironmentEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public async Task<EnvironmentEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new EnvironmentResource
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
        public Task<EnvironmentEditor> CreateOrModify(string name, string description, bool allowDynamicInfrastructure = false)
            => CreateOrModify(name, description, allowDynamicInfrastructure, CancellationToken.None);

        public async Task<EnvironmentEditor> CreateOrModify(string name, string description, bool allowDynamicInfrastructure, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new EnvironmentResource
                {
                    Name = name,
                    Description = description,
                    AllowDynamicInfrastructure = allowDynamicInfrastructure
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;
                existing.AllowDynamicInfrastructure = allowDynamicInfrastructure;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<EnvironmentEditor> CreateOrModify(string name, string description)
            => CreateOrModify(name, description, CancellationToken.None);

        public async Task<EnvironmentEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new EnvironmentResource
                {
                    Name = name,
                    Description = description,
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        public EnvironmentEditor Customize(Action<EnvironmentResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<EnvironmentEditor> Save()
            => Save(CancellationToken.None);

        public async Task<EnvironmentEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
