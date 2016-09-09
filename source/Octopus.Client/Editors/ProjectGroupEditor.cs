using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

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

        public ProjectGroupEditor CreateOrModify(string name)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new ProjectGroupResource
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
        public ProjectGroupEditor CreateOrModify(string name, string description)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new ProjectGroupResource
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

        public ProjectGroupEditor Customize(Action<ProjectGroupResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public ProjectGroupEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}