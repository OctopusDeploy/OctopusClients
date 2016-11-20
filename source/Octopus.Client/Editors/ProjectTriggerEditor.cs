using System;
using System.Linq;
using Octopus.Client.Model;
using Octopus.Client.Model.Triggers;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class ProjectTriggerEditor : IResourceEditor<ProjectTriggerResource, ProjectTriggerEditor>
    {
        private readonly IProjectTriggerRepository repository;

        public ProjectTriggerEditor(IProjectTriggerRepository repository)
        {
            this.repository = repository;
        }

        public ProjectTriggerResource Instance { get; private set; }

        public ProjectTriggerEditor CreateOrModify(ProjectResource project, string name, ProjectTriggerType type, TriggerFilterResource filter, TriggerActionResource action)
        {
            var existing = repository.FindByName(project, name);
            if (existing == null)
            {
                Instance = repository.Create(new ProjectTriggerResource
                {
                    Name = name,
                    ProjectId = project.Id,
                    Type = type,
                    Filter = filter,
                    Action = action
                });
            }
            else
            {
                existing.Name = name;
                existing.Type = type;
                existing.Filter = filter;
                existing.Action = action;
                Instance = repository.Modify(existing);
            }

            return this;
        }

        public ProjectTriggerEditor Customize(Action<ProjectTriggerResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public ProjectTriggerEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}