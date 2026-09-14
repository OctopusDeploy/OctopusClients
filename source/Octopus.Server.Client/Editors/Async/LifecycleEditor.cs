using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class LifecycleEditor : IResourceEditor<LifecycleResource, LifecycleEditor>
    {
        private readonly ILifecyclesRepository repository;

        public LifecycleEditor(ILifecyclesRepository repository)
        {
            this.repository = repository;
        }

        public LifecycleResource Instance { get; private set; }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<LifecycleEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public async Task<LifecycleEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new LifecycleResource
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
        public Task<LifecycleEditor> CreateOrModify(string name, string description)
            => CreateOrModify(name, description, CancellationToken.None);

        public async Task<LifecycleEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new LifecycleResource
                {
                    Name = name,
                    Description = description
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

        public PhaseResource AddOrUpdatePhase(string name)
        {
            return Instance.AddOrUpdatePhase(name);
        }

        public LifecycleEditor AsSimplePromotionLifecycle(IEnumerable<EnvironmentResource> environments)
        {
            Clear();

            foreach (var environment in environments)
            {
                AddOrUpdatePhase(environment.Name).WithOptionalDeploymentTargets(environment);
            }

            return this;
        }

        public LifecycleEditor Clear()
        {
            Instance.Clear();
            return this;
        }

        public LifecycleEditor Customize(Action<LifecycleResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<LifecycleEditor> Save()
            => Save(CancellationToken.None);

        public async Task<LifecycleEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
