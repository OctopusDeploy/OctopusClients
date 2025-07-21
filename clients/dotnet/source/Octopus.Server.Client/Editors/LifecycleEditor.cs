using System;
using System.Collections.Generic;
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

        public LifecycleEditor CreateOrModify(string name)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new LifecycleResource
                {
                    Name = name,
                });
            }
            else
            {
                existing.Name = name;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public LifecycleEditor CreateOrModify(string name, string description)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new LifecycleResource
                {
                    Name = name,
                    Description = description
                });
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = repository.Modify(existing);
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

        public LifecycleEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}