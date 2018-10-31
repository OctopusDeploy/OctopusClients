using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{

    public interface IProjectRepository : IFindByName<ProjectResource>, IGet<ProjectResource>, ICreate<ProjectResource>, IModify<ProjectResource>, IDelete<ProjectResource>, IGetAll<ProjectResource>
    {
        Task<ResourceCollection<ReleaseResource>> GetReleases(ProjectResource project, int skip = 0, int? take = null, string searchByVersion = null);
        Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ProjectResource project);
        Task<ReleaseResource> GetReleaseByVersion(ProjectResource project, string version);
        Task<ResourceCollection<ChannelResource>> GetChannels(ProjectResource project);
        Task<IReadOnlyList<ChannelResource>> GetAllChannels(ProjectResource project);
        Task<ProgressionResource> GetProgression(ProjectResource project);
        Task<ResourceCollection<ProjectTriggerResource>> GetTriggers(ProjectResource project);
        Task<IReadOnlyList<ProjectTriggerResource>> GetAllTriggers(ProjectResource project);
        Task SetLogo(ProjectResource project, string fileName, Stream contents);
        Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle);
        Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId = null);
    }

    class ProjectRepository : BasicRepository<ProjectResource>, IProjectRepository
    {
        public ProjectRepository(IOctopusAsyncRepository repository)
            : base(repository, "Projects")
        {
        }

        public Task<ResourceCollection<ReleaseResource>> GetReleases(ProjectResource project, int skip = 0, int? take = null, string searchByVersion = null)
        {
            return Client.List<ReleaseResource>(project.Link("Releases"), new { skip, take, searchByVersion });
        }

        public Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ProjectResource project) 
        {
            return Client.ListAll<ReleaseResource>(project.Link("Releases"));
        }

        public Task<ReleaseResource> GetReleaseByVersion(ProjectResource project, string version)
        {
            return Client.Get<ReleaseResource>(project.Link("Releases"), new { version });
        }

        public Task<ResourceCollection<ChannelResource>> GetChannels(ProjectResource project)
        {
            return Client.List<ChannelResource>(project.Link("Channels"));
        }

        public Task<IReadOnlyList<ChannelResource>> GetAllChannels(ProjectResource project)
        {
            return Client.ListAll<ChannelResource>(project.Link("Channels"));
        }

        public Task<ProgressionResource> GetProgression(ProjectResource project)
        {
            return Client.Get<ProgressionResource>(project.Link("Progression"));
        }

        public Task<ResourceCollection<ProjectTriggerResource>> GetTriggers(ProjectResource project)
        {
            return Client.List<ProjectTriggerResource>(project.Link("Triggers"));
        }

        public Task<IReadOnlyList<ProjectTriggerResource>> GetAllTriggers(ProjectResource project)
        {
            return Client.ListAll<ProjectTriggerResource>(project.Link("Triggers"));
        }

        public Task SetLogo(ProjectResource project, string fileName, Stream contents)
        {
            return Client.Post(project.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
        }

        public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle)
        {
            return new ProjectEditor(this, new ChannelRepository(Repository), new DeploymentProcessRepository(Repository), new ProjectTriggerRepository(Repository), new VariableSetRepository(Repository)).CreateOrModify(name, projectGroup, lifecycle);
        }

        public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId = null)
        {
            return new ProjectEditor(this, new ChannelRepository(Repository), new DeploymentProcessRepository(Repository), new ProjectTriggerRepository(Repository), new VariableSetRepository(Repository)).CreateOrModify(name, projectGroup, lifecycle, description, cloneId);
        }
    }
}