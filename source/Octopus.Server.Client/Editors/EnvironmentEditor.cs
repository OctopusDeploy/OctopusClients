using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class EnvironmentEditor : IResourceEditor<EnvironmentResource, EnvironmentEditor>
    {
        private readonly IEnvironmentRepository repository;

        public EnvironmentEditor(IEnvironmentRepository repository)
        {
            this.repository = repository;
        }

        public EnvironmentResource Instance { get; private set; }

        public EnvironmentEditor CreateOrModify(string name)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new EnvironmentResource
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

        public EnvironmentEditor CreateOrModify(string name, string description, bool allowDynamicInfrastructure)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new EnvironmentResource
                {
                    Name = name,
                    Description = description,
                    AllowDynamicInfrastructure = allowDynamicInfrastructure
                });
            }
            else
            {
                existing.Name = name;
                existing.Description = description;
                existing.AllowDynamicInfrastructure = allowDynamicInfrastructure;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public EnvironmentEditor CreateOrModify(string name, string description)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new EnvironmentResource
                {
                    Name = name,
                    Description = description,
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

        public EnvironmentEditor Customize(Action<EnvironmentResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public EnvironmentEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}