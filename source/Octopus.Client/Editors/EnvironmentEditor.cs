using System;
using System.Threading.Tasks;
using Octopus.Client.Editors.DeploymentProcess;
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

        public async Task<EnvironmentEditor> CreateOrModify(string name)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new EnvironmentResource
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

        public async Task<EnvironmentEditor> CreateOrModify(string name, string description)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new EnvironmentResource
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

        public EnvironmentEditor Customize(Action<EnvironmentResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<EnvironmentEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}