using System;
using System.Linq;
using Octopus.Client.Model;
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

        public ProjectTriggerEditor CreateOrModify(ProjectResource project, string name, ProjectTriggerType type, params ProjectTriggerConditionEvent[] conditions)
        {
            var conditionsCsv = string.Join(",", conditions.Select(x => x.ToString()).ToArray());

            var existing = repository.FindByName(project, name);
            if (existing == null)
            {
                Instance = repository.Create(new ProjectTriggerResource
                {
                    Name = name,
                    ProjectId = project.Id,
                    Type = type,
                    Properties =
                    {
                        {"Octopus.ProjectTriggerCondition.Events", new PropertyValueResource(conditionsCsv)}
                    }
                });
            }
            else
            {
                existing.Name = name;
                existing.Type = type;
                existing.Properties["Octopus.ProjectTriggerCondition.Events"] = new PropertyValueResource(conditionsCsv);
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