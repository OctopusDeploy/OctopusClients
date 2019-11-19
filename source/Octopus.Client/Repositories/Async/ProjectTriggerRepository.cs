using System;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;
using Octopus.Client.Model.Triggers;

namespace Octopus.Client.Repositories.Async
{
    public interface IProjectTriggerRepository : ICreate<ProjectTriggerResource>, IModify<ProjectTriggerResource>, IGet<ProjectTriggerResource>, IDelete<ProjectTriggerResource>
    {
        Task<ProjectTriggerResource> FindByName(ProjectResource project, string name);

        Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action);
    }

    class ProjectTriggerRepository : BasicRepository<ProjectTriggerResource>, IProjectTriggerRepository
    {
        public ProjectTriggerRepository(IOctopusAsyncRepository repository)
            : base(repository, "ProjectTriggers")
        {
        }

        async Task CheckServerVersionSupportsNewTriggerModel()
        {
            var versionWhenScheduledTriggersWereChanged = SemanticVersion.Parse("2019.11.1");
            var rootDocument = await Repository.LoadRootDocument().ConfigureAwait(false);
            if (SemanticVersion.Parse(rootDocument.Version) < versionWhenScheduledTriggersWereChanged)
            {
                throw new NotSupportedException("The version of the Octopus Server you are connecting to is not compatible with this version of Octopus.Client. Pleas upgrade your Octopus Server to a version >= 2019.11.1");
            }
        }

        public async Task<ProjectTriggerResource> FindByName(ProjectResource project, string name)
        {
            await CheckServerVersionSupportsNewTriggerModel();
            return await FindByName(name, path: project.Link("Triggers"));
        }

        public async Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action)
        {
            await CheckServerVersionSupportsNewTriggerModel();
            return await new ProjectTriggerEditor(this).CreateOrModify(project, name, filter, action);
        }
    }
}
