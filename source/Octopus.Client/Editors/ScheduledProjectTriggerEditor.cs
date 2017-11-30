using System;
using Octopus.Client.Model;
using Octopus.Client.Model.ScheduledTriggers;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class ScheduledProjectTriggerEditor : IResourceEditor<ScheduledProjectTriggerResource, ScheduledProjectTriggerEditor>
    {
        private readonly IScheduledProjectTriggerRepository repository;

        public ScheduledProjectTriggerEditor(IScheduledProjectTriggerRepository repository)
        {
            this.repository = repository;
        }

        public ScheduledProjectTriggerResource Instance { get; private set; }

        public ScheduledProjectTriggerEditor CreateOrModify(ProjectResource project, string name, TriggerScheduleResource schedule, ScheduledTriggerActionResource action)
        {
            var existing = repository.FindByName(project, name);
            if (existing == null)
            {
                Instance = repository.Create(new ScheduledProjectTriggerResource
                {
                    Name = name,
                    ProjectId = project.Id,
                    Schedule = schedule,
                    Action = action
                });
            }
            else
            {
                existing.Name = name;
                existing.Schedule = schedule;
                existing.Action = action;
                Instance = repository.Modify(existing);
            }

            return this;
        }

        public ScheduledProjectTriggerEditor Customize(Action<ScheduledProjectTriggerResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public ScheduledProjectTriggerEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}
