using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.ScheduledTriggers;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class ScheduledProjectTriggerEditor : IResourceEditor<ScheduledProjectTriggerResource, ScheduledProjectTriggerEditor>
    {
        private readonly IScheduledProjectTriggerRepository repository;

        public ScheduledProjectTriggerEditor(IScheduledProjectTriggerRepository repository)
        {
            this.repository = repository;
        }

        public ScheduledProjectTriggerResource Instance { get; private set; }

        public async Task<ScheduledProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerScheduleResource schedule, ScheduledTriggerActionResource action)
        {
            var existing = await repository.FindByName(project, name);
            if (existing == null)
            {
                Instance = await repository.Create(new ScheduledProjectTriggerResource
                {
                    Name = name,
                    ProjectId = project.Id,
                    Schedule = schedule,
                    Action = action
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Schedule = schedule;
                existing.Action = action;
                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public ScheduledProjectTriggerEditor Customize(Action<ScheduledProjectTriggerResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<ScheduledProjectTriggerEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}
