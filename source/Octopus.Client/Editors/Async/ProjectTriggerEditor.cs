using System;
using System.Linq;
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

        public async Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, ProjectTriggerType type, params ProjectTriggerConditionEvent[] conditions)
        {
            var conditionsCsv = string.Join(",", conditions.Select(x => x.ToString()).ToArray());

            var existing = await repository.FindByName(project, name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new ProjectTriggerResource
                {
                    Name = name,
                    ProjectId = project.Id,
                    Type = type,
                    Properties =
                    {
                        {"Octopus.ProjectTriggerCondition.Events", new PropertyValueResource(conditionsCsv)}
                    }
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Type = type;
                existing.Properties["Octopus.ProjectTriggerCondition.Events"] = new PropertyValueResource(conditionsCsv);
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