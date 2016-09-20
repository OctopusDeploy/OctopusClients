using System;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    class ProjectsRepository : BasicRepository<ProjectResource>, IProjectRepository
    {
        public ProjectsRepository(IOctopusAsyncClient client)
            : base(client, "Projects")
        {
        }

        public Task<ResourceCollection<ReleaseResource>> GetReleases(ProjectResource project, int skip = 0)
        {
            return Client.List<ReleaseResource>(project.Link("Releases"), new { skip });
        }

        public Task<ReleaseResource> GetReleaseByVersion(ProjectResource project, string version)
        {
            return Client.Get<ReleaseResource>(project.Link("Releases"), new { version });
        }

        public Task<ResourceCollection<ChannelResource>> GetChannels(ProjectResource project)
        {
            return Client.List<ChannelResource>(project.Link("Channels"));
        }

        public Task<ResourceCollection<ProjectTriggerResource>> GetTriggers(ProjectResource project)
        {
            return Client.List<ProjectTriggerResource>(project.Link("Triggers"));
        }

        public Task SetLogo(ProjectResource project, string fileName, Stream contents)
        {
            return Client.Post(project.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
        }

        public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle)
        {
            return new ProjectEditor(this, new ChannelRepository(Client), new DeploymentProcessRepository(Client), new ProjectTriggerRepository(Client), new VariableSetRepository(Client)).CreateOrModify(name, projectGroup, lifecycle);
        }

        public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description)
        {
            return new ProjectEditor(this, new ChannelRepository(Client), new DeploymentProcessRepository(Client), new ProjectTriggerRepository(Client), new VariableSetRepository(Client)).CreateOrModify(name, projectGroup, lifecycle, description);
        }
    }
}