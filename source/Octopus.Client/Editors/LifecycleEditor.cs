using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.DeploymentProcess;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class LifecycleEditor : IResourceEditor<LifecycleResource, LifecycleEditor>
    {
        private readonly ILifecyclesRepository repository;

        public LifecycleEditor(ILifecyclesRepository repository)
        {
            this.repository = repository;
        }

        public LifecycleResource Instance { get; private set; }

        public async Task<LifecycleEditor> CreateOrModify(string name)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new LifecycleResource
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

        public async Task<LifecycleEditor> CreateOrModify(string name, string description)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new LifecycleResource
                {
                    Name = name,
                    Description = description
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
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

        public async Task<LifecycleEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}