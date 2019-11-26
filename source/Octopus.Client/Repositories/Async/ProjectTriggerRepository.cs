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
        Task<ResourceCollection<ProjectTriggerResource>> FindByRunbook(params string[] runbookIds);
    }

    class ProjectTriggerRepository : BasicRepository<ProjectTriggerResource>, IProjectTriggerRepository
    {
        public ProjectTriggerRepository(IOctopusAsyncRepository repository)
            : base(repository, "ProjectTriggers")
        {
            MinimumCompatibleVersion("2019.11.0");
        }

        public Task<ProjectTriggerResource> FindByName(ProjectResource project, string name)
        {
            ThrowIfServerVersionIsNotCompatible();
            
            return FindByName(name, path: project.Link("Triggers"));
        }

        public Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action)
        {
            ThrowIfServerVersionIsNotCompatible();
            
            return new ProjectTriggerEditor(this).CreateOrModify(project, name, filter, action);
        }

        public async Task<ResourceCollection<ProjectTriggerResource>> FindByRunbook(params string[] runbookIds)
        {
            return await Client.List<ProjectTriggerResource>(await Repository.Link("Triggers"), new { runbooks = runbookIds });
        }

    }
}
