using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.ScheduledTriggers;
using Octopus.Client.Model.Triggers;

namespace Octopus.Client.Repositories
{
    public interface IScheduledProjectTriggerRepository : ICreate<ScheduledProjectTriggerResource>, IModify<ScheduledProjectTriggerResource>, IGet<ScheduledProjectTriggerResource>, IDelete<ScheduledProjectTriggerResource>
    {
        ScheduledProjectTriggerResource FindByName(ProjectResource project, string name);
        ScheduledProjectTriggerEditor CreateOrModify(ProjectResource project, string name, TriggerScheduleResource schedule, ScheduledTriggerActionResource action);
    }

    internal class ScheduledProjectTriggerRepository : BasicRepository<ScheduledProjectTriggerResource>, IScheduledProjectTriggerRepository
    {
        public ScheduledProjectTriggerRepository(IOctopusClient client)
            : base(client, "ScheduledProjectTriggers")
        {
        }

        public ScheduledProjectTriggerResource FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("ScheduledTriggers"));
        }

        public ScheduledProjectTriggerEditor CreateOrModify(ProjectResource project, string name, TriggerScheduleResource schedule, ScheduledTriggerActionResource action)
        {
            return new ScheduledProjectTriggerEditor(this).CreateOrModify(project, name, schedule, action);
        }
    }
}
