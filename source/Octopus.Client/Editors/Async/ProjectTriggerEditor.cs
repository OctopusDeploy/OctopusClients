using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class ProjectTriggerEditor : IResourceEditor<ProjectTriggerResource, ProjectTriggerEditor>
    {
        private readonly IProjectTriggerRepository repository;

        public ProjectTriggerEditor(IProjectTriggerRepository repository)
        {
            this.repository = repository;
        }

        public ProjectTriggerResource Instance { get; private set; }

        public async Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, ProjectTriggerType type, IProjectTriggerFilterResource filter, IProjectTriggerActionResource action)
        {
            var existing = await repository.FindByName(project, name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new ProjectTriggerResource
                {
                    Name = name,
                    ProjectId = project.Id,
                    Type = type,
                    Filter = filter,
                    Action = action
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Type = type;
                existing.Filter = filter;
                existing.Action = action;
                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public ProjectTriggerEditor Customize(Action<ProjectTriggerResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<ProjectTriggerEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}