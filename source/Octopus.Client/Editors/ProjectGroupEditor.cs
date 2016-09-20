using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors
{
    public class ProjectGroupEditor : IResourceEditor<ProjectGroupResource, ProjectGroupEditor>
    {
        private readonly IProjectGroupRepository repository;

        public ProjectGroupEditor(IProjectGroupRepository repository)
        {
            this.repository = repository;
        }

        public ProjectGroupResource Instance { get; private set; }

        public async Task<ProjectGroupEditor> CreateOrModify(string name)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new ProjectGroupResource
                {
                    Name = name,
                });
            }
            else
            {
                existing.Name = name;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }
        public async Task<ProjectGroupEditor> CreateOrModify(string name, string description)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new ProjectGroupResource
                {
                    Name = name,
                    Description = description
                });
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public ProjectGroupEditor Customize(Action<ProjectGroupResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<ProjectGroupEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}