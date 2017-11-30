using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;
using Octopus.Client.Model.ScheduledTriggers;

namespace Octopus.Client.Repositories.Async
{
    public interface IScheduledProjectTriggerRepository : ICreate<ScheduledProjectTriggerResource>, IModify<ScheduledProjectTriggerResource>, IGet<ScheduledProjectTriggerResource>, IDelete<ScheduledProjectTriggerResource>
    {
        Task<ScheduledProjectTriggerResource> FindByName(ProjectResource project, string name);

        Task<ScheduledProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerScheduleResource filter, ScheduledTriggerActionResource action);
    }

    class ScheduledProjectTriggerRepository : BasicRepository<ScheduledProjectTriggerResource>, IScheduledProjectTriggerRepository
    {
        public ScheduledProjectTriggerRepository(IOctopusAsyncClient client)
            : base(client, "ScheduledProjectTriggers")
        {
        }

        public Task<ScheduledProjectTriggerResource> FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Triggers"));
        }

        public Task<ScheduledProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerScheduleResource filter, ScheduledTriggerActionResource action)
        {
            return new ScheduledProjectTriggerEditor(this).CreateOrModify(project, name, filter, action);
        }
    }
}
