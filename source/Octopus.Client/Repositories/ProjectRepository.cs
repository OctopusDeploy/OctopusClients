using System;
using System.Collections.Generic;
using System.IO;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IProjectRepository : IFindByName<ProjectResource>, IGet<ProjectResource>, ICreate<ProjectResource>, IModify<ProjectResource>, IDelete<ProjectResource>, IGetAll<ProjectResource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <returns></returns>
        ResourceCollection<ReleaseResource> GetReleases(ProjectResource project, int skip = 0, int? take = null);
        IReadOnlyList<ReleaseResource> GetAllReleases(ProjectResource project);
        ReleaseResource GetReleaseByVersion(ProjectResource project, string version);
        ResourceCollection<ChannelResource> GetChannels(ProjectResource project);
        ProgressionResource GetProgression(ProjectResource project);
        ResourceCollection<ProjectTriggerResource> GetTriggers(ProjectResource project);
        void SetLogo(ProjectResource project, string fileName, Stream contents);
        ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle);
        ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description);
    }
    
    class ProjectRepository : BasicRepository<ProjectResource>, IProjectRepository
    {
        public ProjectRepository(IOctopusClient client)
            : base(client, "Projects")
        {
        }

        public ResourceCollection<ReleaseResource> GetReleases(ProjectResource project, int skip = 0, int? take = null)
        {
            return Client.List<ReleaseResource>(project.Link("Releases"), new { skip, take });
        }

        public IReadOnlyList<ReleaseResource> GetAllReleases(ProjectResource project)
        {
            return Client.ListAll<ReleaseResource>(project.Link("Releases"));
        }

        public ReleaseResource GetReleaseByVersion(ProjectResource project, string version)
        {
            return Client.Get<ReleaseResource>(project.Link("Releases"), new { version });
        }

        public ResourceCollection<ChannelResource> GetChannels(ProjectResource project)
        {
            return Client.List<ChannelResource>(project.Link("Channels"));
        }

        public ProgressionResource GetProgression(ProjectResource project)
        {
            return Client.Get<ProgressionResource>(project.Link("Progression"));
        }

        public ResourceCollection<ProjectTriggerResource> GetTriggers(ProjectResource project)
        {
            return Client.List<ProjectTriggerResource>(project.Link("Triggers"));
        }

        public void SetLogo(ProjectResource project, string fileName, Stream contents)
        {
            Client.Post(project.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
        }

        public ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle)
        {
            return new ProjectEditor(this, new ChannelRepository(Client), new DeploymentProcessRepository(Client), new ProjectTriggerRepository(Client), new VariableSetRepository(Client)).CreateOrModify(name, projectGroup, lifecycle);
        }

        public ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description)
        {
            return new ProjectEditor(this, new ChannelRepository(Client), new DeploymentProcessRepository(Client), new ProjectTriggerRepository(Client), new VariableSetRepository(Client)).CreateOrModify(name, projectGroup, lifecycle, description);
        }
    }
}