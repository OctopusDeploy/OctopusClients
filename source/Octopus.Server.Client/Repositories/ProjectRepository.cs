using System;
using System.Collections.Generic;
using System.IO;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Git;

namespace Octopus.Client.Repositories
{
    public interface IProjectRepository : IFindByName<ProjectResource>, IGet<ProjectResource>, ICreate<ProjectResource>, IModify<ProjectResource>, IDelete<ProjectResource>, IGetAll<ProjectResource>
    {
        
        ResourceCollection<GitTagResource> GetGitTags(ProjectResource projectResource);
        GitTagResource GetGitTag(ProjectResource projectResource, string tag);
        ResourceCollection<GitBranchResource> GetGitBranches(ProjectResource projectResource);
        GitBranchResource GetGitBranch(ProjectResource projectResource, string branch);
        GitCommitResource GetGitCommit(ProjectResource projectResource, string hash);
        ConvertProjectToGitResponse ConvertToGit(ProjectResource project, GitPersistenceSettingsResource gitPersistenceSettings, string commitMessage);
        ConvertProjectToGitResponse ConvertToGit(ProjectResource project, GitPersistenceSettingsResource gitPersistenceSettings, string commitMessage, string initialCommitBranch);
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
        IReadOnlyList<RunbookResource> GetAllRunbooks(ProjectResource project, string gitRef);
    }

    class ProjectRepository : BasicRepository<ProjectResource>, IProjectRepository
    {

        public ProjectRepository(IOctopusRepository repository)
            : base(repository, "Projects")
        {
        }
        
        public ResourceCollection<GitBranchResource> GetGitBranches(ProjectResource projectResource)
        {
            if (!projectResource.IsVersionControlled)
                throw new NotSupportedException($"Database backed projects do not support branches");
            
            return Client.Get<ResourceCollection<GitBranchResource>>(projectResource.Link("Branches"));
        }
        
        public ResourceCollection<GitTagResource> GetGitTags(ProjectResource projectResource)
        {
            if (!projectResource.IsVersionControlled)
                throw new NotSupportedException($"Database backed projects do not support tags");
            
            return Client.Get<ResourceCollection<GitTagResource>>(projectResource.Link("Tags"));
        }

        public GitBranchResource GetGitBranch(ProjectResource projectResource, string branch)
        {
            if (!projectResource.IsVersionControlled)
                throw new NotSupportedException($"Database backed projects do not support branches");

            return Client.Get<GitBranchResource>(projectResource.Link("Branches"), new {name = branch});
        }

        public GitCommitResource GetGitCommit(ProjectResource projectResource, string hash)
        {
            if (!projectResource.IsVersionControlled)
                throw new NotSupportedException($"Database backed projects do not support commits");

            return Client.Get<GitCommitResource>(projectResource.Link("Commits"), new { hash });
        }

        public GitTagResource GetGitTag(ProjectResource projectResource, string tag)
        {
            if (!projectResource.IsVersionControlled)
                throw new NotSupportedException($"Database backed projects do not support tags");

            return Client.Get<GitTagResource>(projectResource.Link("Tags"), new {name = tag});
        }

        public ConvertProjectToGitResponse ConvertToGit(ProjectResource project,
            GitPersistenceSettingsResource gitPersistenceSettings, string commitMessage)
        {
            return ConvertToGit(project, gitPersistenceSettings, commitMessage, null);
        }

        public ConvertProjectToGitResponse ConvertToGit(ProjectResource project,
            GitPersistenceSettingsResource gitPersistenceSettings, string commitMessage, string initialCommitBranch)
        {
            var payload = new ConvertProjectToGitCommand
            {
                VersionControlSettings = gitPersistenceSettings,
                CommitMessage = commitMessage,
                InitialCommitBranchName = initialCommitBranch
            };

            var url = project.HasLink("ConvertToGit") ? project.Link("ConvertToGit") : project.Link("ConvertToVcs");
            var response =
                Client.Post<ConvertProjectToGitCommand, ConvertProjectToGitResponse>(url,
                    payload);
            return response;
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

        public IReadOnlyList<RunbookResource> GetAllRunbooks(ProjectResource project, string gitRef) => Client.ListAll<RunbookResource>(project.Link("Runbooks"), new { gitRef });
    }
}
