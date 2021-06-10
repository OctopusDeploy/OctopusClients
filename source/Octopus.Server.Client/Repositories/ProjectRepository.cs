using System;
using System.Collections.Generic;
using System.IO;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.VersionControl;

namespace Octopus.Client.Repositories
{
    public interface IProjectRepository : IFindByName<ProjectResource>, IGet<ProjectResource>, ICreate<ProjectResource>, IModify<ProjectResource>, IDelete<ProjectResource>, IGetAll<ProjectResource>
    {
        IProjectBetaRepository Beta();
        ResourceCollection<ReleaseResource> GetReleases(ProjectResource project, int skip = 0, int? take = null, string searchByVersion = null);
        IReadOnlyList<ReleaseResource> GetAllReleases(ProjectResource project);
        ReleaseResource GetReleaseByVersion(ProjectResource project, string version);
        ResourceCollection<ChannelResource> GetChannels(ProjectResource project);
        IReadOnlyList<ChannelResource> GetAllChannels(ProjectResource project);
        ProgressionResource GetProgression(ProjectResource project);
        ResourceCollection<ProjectTriggerResource> GetTriggers(ProjectResource project);
        IReadOnlyList<ProjectTriggerResource> GetAllTriggers(ProjectResource project);
        void SetLogo(ProjectResource project, string fileName, Stream contents);
        ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle);
        ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId = null);
        ResourceCollection<RunbookSnapshotResource> GetRunbookSnapshots(ProjectResource project, int skip = 0, int? take = null, string searchByName = null);
        IReadOnlyList<RunbookSnapshotResource> GetAllRunbookSnapshots(ProjectResource project);
        RunbookSnapshotResource GetRunbookSnapshotByName(ProjectResource project, string name);
        ResourceCollection<RunbookResource> GetRunbooks(ProjectResource project, int skip = 0, int? take = null, string searchByName = null);
        IReadOnlyList<RunbookResource> GetAllRunbooks(ProjectResource project);
    }

    class ProjectRepository : BasicRepository<ProjectResource>, IProjectRepository
    {
        private readonly IProjectBetaRepository beta;

        public ProjectRepository(IOctopusRepository repository)
            : base(repository, "Projects")
        {
            beta = new ProjectBetaRepository(repository);
        }

        public IProjectBetaRepository Beta()
        {
            return beta;
        }

        public ResourceCollection<ReleaseResource> GetReleases(ProjectResource project, int skip = 0, int? take = null, string searchByVersion = null)
        {
            return Client.List<ReleaseResource>(project.Link("Releases"), new { skip, take, searchByVersion });
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

        public IReadOnlyList<ChannelResource> GetAllChannels(ProjectResource project)
        {
            return Client.ListAll<ChannelResource>(project.Link("Channels"));
        }

        public ProgressionResource GetProgression(ProjectResource project)
        {
            return Client.Get<ProgressionResource>(project.Link("Progression"));
        }

        public ResourceCollection<ProjectTriggerResource> GetTriggers(ProjectResource project)
        {
            return Client.List<ProjectTriggerResource>(project.Link("Triggers"));
        }

        public IReadOnlyList<ProjectTriggerResource> GetAllTriggers(ProjectResource project)
        {
            return Client.ListAll<ProjectTriggerResource>(project.Link("Triggers"));
        }

        public void SetLogo(ProjectResource project, string fileName, Stream contents)
        {
            Client.Post(project.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
        }

        public ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle)
        {
            return new ProjectEditor(this, new ChannelRepository(Repository), new DeploymentProcessRepository(Repository), new ProjectTriggerRepository(Repository), new VariableSetRepository(Repository)).CreateOrModify(name, projectGroup, lifecycle);
        }

        public ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId = null)
        {
            return new ProjectEditor(this, new ChannelRepository(Repository), new DeploymentProcessRepository(Repository), new ProjectTriggerRepository(Repository), new VariableSetRepository(Repository)).CreateOrModify(name, projectGroup, lifecycle, description, cloneId);
        }

        public ResourceCollection<RunbookSnapshotResource> GetRunbookSnapshots(ProjectResource project, int skip = 0, int? take = null, string searchByName = null)
        {
            return Client.List<RunbookSnapshotResource>(project.Link("RunbookSnapshots"), new { skip, take, searchByName });
        }

        public IReadOnlyList<RunbookSnapshotResource> GetAllRunbookSnapshots(ProjectResource project)
        {
            return Client.ListAll<RunbookSnapshotResource>(project.Link("RunbookSnapshots"));
        }

        public RunbookSnapshotResource GetRunbookSnapshotByName(ProjectResource project, string name)
        {
            return Client.Get<RunbookSnapshotResource>(project.Link("RunbookSnapshots"), new { name });
        }

        public ResourceCollection<RunbookResource> GetRunbooks(ProjectResource project, int skip = 0, int? take = null, string searchByName = null)
        {
            return Client.List<RunbookResource>(project.Link("Runbooks"), new { skip, take, searchByName });
        }

        public IReadOnlyList<RunbookResource> GetAllRunbooks(ProjectResource project)
        {
            return Client.ListAll<RunbookResource>(project.Link("Runbooks"));
        }
    }

    public interface IProjectBetaRepository
    {
        ResourceCollection<VersionControlBranchResource> GetVersionControlledBranches(ProjectResource projectResource);
        VersionControlBranchResource GetVersionControlledBranch(ProjectResource projectResource, string branch);
        ConvertProjectToVersionControlledResponse ConvertToVersionControlled(ProjectResource project, VersionControlSettingsResource versionControlSettings, string commitMessage);
        ResourceCollection<ChannelResource> GetChannels(ProjectResource projectResource, string gitRef);
        IReadOnlyList<ChannelResource> GetAllChannels(ProjectResource projectResource, string gitRef);
        ChannelResource GetChannel(ProjectResource projectResource, string gitRef, string idOrName);
    }

    internal class ProjectBetaRepository : IProjectBetaRepository
    {
        private readonly IOctopusClient client;

        public ProjectBetaRepository(IOctopusRepository repository)
        {
            client = repository.Client;
        }

        public ResourceCollection<VersionControlBranchResource> GetVersionControlledBranches(ProjectResource projectResource)
        {
            return client.Get<ResourceCollection<VersionControlBranchResource>>(projectResource.Link("Branches"));
        }

        public VersionControlBranchResource GetVersionControlledBranch(ProjectResource projectResource, string branch)
        {
            return client.Get<VersionControlBranchResource>(projectResource.Link("Branches"), new {name = branch});
        }

        public ConvertProjectToVersionControlledResponse ConvertToVersionControlled(ProjectResource project,
            VersionControlSettingsResource versionControlSettings, string commitMessage)
        {
            var payload = new ConvertProjectToVersionControlledCommand
            {
                VersionControlSettings = versionControlSettings,
                CommitMessage = commitMessage
            };

            var url = project.Link("ConvertToVcs");
            var response =
                client.Post<ConvertProjectToVersionControlledCommand, ConvertProjectToVersionControlledResponse>(url,
                    payload);
            return response;
        }

        public ResourceCollection<ChannelResource> GetChannels(ProjectResource projectResource, string gitRef)
        {
            projectResource.EnsureVersionControlled();
            VersionControlBranchResource branch = GetVersionControlledBranch(projectResource, gitRef);
            return client.List<ChannelResource>(branch.Link("Channels"));
        }

        public IReadOnlyList<ChannelResource> GetAllChannels(ProjectResource projectResource, string gitRef)
        {
            projectResource.EnsureVersionControlled();
            VersionControlBranchResource branch = GetVersionControlledBranch(projectResource, gitRef);
            return client.ListAll<ChannelResource>(branch.Link("Channels"));
        }

        public ChannelResource GetChannel(ProjectResource projectResource, string gitRef, string idOrName)
        {
            projectResource.EnsureVersionControlled();

            VersionControlBranchResource branch = GetVersionControlledBranch(projectResource, gitRef);
            var url = $"{branch.Link("Channels")}/{idOrName}";

            return client.Get<ChannelResource>(url);
        }
    }
}