using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;
using Octopus.Client.Model.Git;

namespace Octopus.Client.Repositories.Async
{
    public interface IProjectRepository : IFindByName<ProjectResource>, IGet<ProjectResource>, ICreate<ProjectResource>, IModify<ProjectResource>, IDelete<ProjectResource>, IGetAll<ProjectResource>
    {
        IProjectBetaRepository Beta();
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<GitBranchResource>> GetGitBranches(ProjectResource projectResource);
        Task<ResourceCollection<GitBranchResource>> GetGitBranches(ProjectResource projectResource, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<GitBranchResource> GetGitBranch(ProjectResource projectResource, string branch);
        Task<GitBranchResource> GetGitBranch(ProjectResource projectResource, string branch, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<GitTagResource>> GetGitTags(ProjectResource projectResource);
        Task<ResourceCollection<GitTagResource>> GetGitTags(ProjectResource projectResource, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<GitTagResource> GetGitTag(ProjectResource projectResource, string tag);
        Task<GitTagResource> GetGitTag(ProjectResource projectResource, string tag, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<GitCommitResource> GetGitCommit(ProjectResource projectResource, string hash);
        Task<GitCommitResource> GetGitCommit(ProjectResource projectResource, string hash, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ConvertProjectToGitResponse> ConvertToGit(ProjectResource project, GitPersistenceSettingsResource gitPersistenceSettings, string commitMessage);
        Task<ConvertProjectToGitResponse> ConvertToGit(ProjectResource project, GitPersistenceSettingsResource gitPersistenceSettings, string commitMessage, CancellationToken cancellationToken);
        Task<ConvertProjectToGitResponse> ConvertToGit(ProjectResource project, GitPersistenceSettingsResource gitPersistenceSettings, string commitMessage, string initialCommitBranch, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<ReleaseResource>> GetReleases(ProjectResource project, int skip = 0, int? take = null, string searchByVersion = null);
        Task<ResourceCollection<ReleaseResource>> GetReleases(ProjectResource project, CancellationToken cancellationToken);
        Task<ResourceCollection<ReleaseResource>> GetReleases(ProjectResource project, int skip, int? take, string searchByVersion, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ProjectResource project);
        Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ProjectResource project, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ReleaseResource> GetReleaseByVersion(ProjectResource project, string version);
        Task<ReleaseResource> GetReleaseByVersion(ProjectResource project, string version, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<ChannelResource>> GetChannels(ProjectResource project);
        Task<ResourceCollection<ChannelResource>> GetChannels(ProjectResource project, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<IReadOnlyList<ChannelResource>> GetAllChannels(ProjectResource project);
        Task<IReadOnlyList<ChannelResource>> GetAllChannels(ProjectResource project, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ProgressionResource> GetProgression(ProjectResource project);
        Task<ProgressionResource> GetProgression(ProjectResource project, CancellationToken cancellationToken, int? releaseHistoryCount = null);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<ProjectTriggerResource>> GetTriggers(ProjectResource project);
        Task<ResourceCollection<ProjectTriggerResource>> GetTriggers(ProjectResource project, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<IReadOnlyList<ProjectTriggerResource>> GetAllTriggers(ProjectResource project);
        Task<IReadOnlyList<ProjectTriggerResource>> GetAllTriggers(ProjectResource project, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task SetLogo(ProjectResource project, string fileName, Stream contents);
        Task SetLogo(ProjectResource project, string fileName, Stream contents, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle);
        Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId = null);
        Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, CancellationToken cancellationToken);
        Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<RunbookSnapshotResource>> GetRunbookSnapshots(ProjectResource project, int skip = 0, int? take = null, string searchByName = null);
        Task<ResourceCollection<RunbookSnapshotResource>> GetRunbookSnapshots(ProjectResource project, CancellationToken cancellationToken);
        Task<ResourceCollection<RunbookSnapshotResource>> GetRunbookSnapshots(ProjectResource project, int skip, int? take, string searchByName, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<IReadOnlyList<RunbookSnapshotResource>> GetAllRunbookSnapshots(ProjectResource project);
        Task<IReadOnlyList<RunbookSnapshotResource>> GetAllRunbookSnapshots(ProjectResource project, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<RunbookSnapshotResource> GetRunbookSnapshotByName(ProjectResource project, string name);
        Task<RunbookSnapshotResource> GetRunbookSnapshotByName(ProjectResource project, string name, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<RunbookResource>> GetRunbooks(ProjectResource project, int skip = 0, int? take = null, string searchByName = null);
        Task<ResourceCollection<RunbookResource>> GetRunbooks(ProjectResource project, CancellationToken cancellationToken);
        Task<ResourceCollection<RunbookResource>> GetRunbooks(ProjectResource project, int skip, int? take, string searchByName, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<IReadOnlyList<RunbookResource>> GetAllRunbooks(ProjectResource project);
        Task<IReadOnlyList<RunbookResource>> GetAllRunbooks(ProjectResource project, CancellationToken cancellationToken);
    }

    class ProjectRepository : BasicRepository<ProjectResource>, IProjectRepository
    {
        readonly IProjectBetaRepository beta;

        public ProjectRepository(IOctopusAsyncRepository repository)
            : base(repository, "Projects")
        {
            beta = new ProjectBetaRepository(repository);
        }

        public IProjectBetaRepository Beta() => beta;

        public Task<ResourceCollection<GitBranchResource>> GetGitBranches(ProjectResource projectResource)
        {
            return GetGitBranches(projectResource, CancellationToken.None);
        }
        
        public Task<ResourceCollection<GitBranchResource>> GetGitBranches(ProjectResource projectResource, CancellationToken cancellationToken)
        {
            if (!projectResource.IsVersionControlled)
                throw new NotSupportedException($"Database backed projects do not support branches");
            
            return Client.Get<ResourceCollection<GitBranchResource>>(projectResource.Link("Branches"), cancellationToken);
        }

        public Task<GitBranchResource> GetGitBranch(ProjectResource projectResource, string branch)
        {
            return GetGitBranch(projectResource, branch, CancellationToken.None);
        }
        
        public Task<GitBranchResource> GetGitBranch(ProjectResource projectResource, string branch, CancellationToken cancellationToken)
        {
            if (!projectResource.IsVersionControlled)
                throw new NotSupportedException($"Database backed projects do not support branches");

            return Client.Get<GitBranchResource>(projectResource.Link("Branches"), new { name = branch }, cancellationToken);
        }

        public Task<ResourceCollection<GitTagResource>> GetGitTags(ProjectResource projectResource)
        {
            return GetGitTags(projectResource, CancellationToken.None);
        }

        public Task<ResourceCollection<GitTagResource>> GetGitTags(ProjectResource projectResource, CancellationToken cancellationToken)
        {
            if (!projectResource.IsVersionControlled)
                throw new NotSupportedException($"Database backed projects do not support branches");
            
            return Client.Get<ResourceCollection<GitTagResource>>(projectResource.Link("Tags"), cancellationToken);
        }
        
        public Task<GitTagResource> GetGitTag(ProjectResource projectResource, string branch)
        {
            return GetGitTag(projectResource, branch, CancellationToken.None);
        }

        public Task<GitTagResource> GetGitTag(ProjectResource projectResource, string branch, CancellationToken cancellationToken)
        {
            if (!projectResource.IsVersionControlled)
                throw new NotSupportedException($"Database backed projects do not support branches");

            return Client.Get<GitTagResource>(projectResource.Link("Tags"), new { name = branch }, cancellationToken);
        }

        public Task<GitCommitResource> GetGitCommit(ProjectResource projectResource, string hash)
        {
            return GetGitCommit(projectResource, hash, CancellationToken.None);
        }

        public Task<GitCommitResource> GetGitCommit(ProjectResource projectResource, string hash, CancellationToken cancellationToken)
        {
            if (!projectResource.IsVersionControlled)
                throw new NotSupportedException($"Database backed projects do not support commits");

            return Client.Get<GitCommitResource>(projectResource.Link("Commits"), new { hash }, cancellationToken);
        }

        public async Task<ConvertProjectToGitResponse> ConvertToGit(ProjectResource project, GitPersistenceSettingsResource gitPersistenceSettings, string commitMessage)
        {
            return await ConvertToGit(project, gitPersistenceSettings, commitMessage, CancellationToken.None);
        }

        public async Task<ConvertProjectToGitResponse> ConvertToGit(ProjectResource project, GitPersistenceSettingsResource gitPersistenceSettings, string commitMessage, CancellationToken cancellationToken)
        {
            return await ConvertToGit(project, gitPersistenceSettings, commitMessage, null, cancellationToken);
        }

        public async Task<ConvertProjectToGitResponse> ConvertToGit(ProjectResource project, GitPersistenceSettingsResource gitPersistenceSettings, string commitMessage, string initialCommitBranch, CancellationToken cancellationToken)
        {
            var payload = new ConvertProjectToGitCommand
            {
                VersionControlSettings = gitPersistenceSettings,
                CommitMessage = commitMessage,
                InitialCommitBranchName = initialCommitBranch
            };

            var url = project.HasLink("ConvertToGit") ? project.Link("ConvertToGit") : project.Link("ConvertToVcs");
            var response = await Client.Post<ConvertProjectToGitCommand,ConvertProjectToGitResponse>(url, payload, cancellationToken);
            return response;
        }

        public Task<ResourceCollection<ReleaseResource>> GetReleases(ProjectResource project, int skip = 0, int? take = null, string searchByVersion = null)
        {
            return GetReleases(project, skip, take, searchByVersion, CancellationToken.None);
        }

        public Task<ResourceCollection<ReleaseResource>> GetReleases(ProjectResource project, CancellationToken cancellationToken)
        {
            return GetReleases(project, 0, null, null, cancellationToken);
        }

        public Task<ResourceCollection<ReleaseResource>> GetReleases(ProjectResource project, int skip, int? take, string searchByVersion, CancellationToken cancellationToken)
        {
            return Client.List<ReleaseResource>(project.Link("Releases"), new { skip, take, searchByVersion }, cancellationToken);
        }

        public Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ProjectResource project)
        {
            return GetAllReleases(project, CancellationToken.None);
        }

        public Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ProjectResource project, CancellationToken cancellationToken)
        {
            return Client.ListAll<ReleaseResource>(project.Link("Releases"), cancellationToken);
        }

        public Task<ReleaseResource> GetReleaseByVersion(ProjectResource project, string version)
        {
            return GetReleaseByVersion(project, version, CancellationToken.None);
        }

        public Task<ReleaseResource> GetReleaseByVersion(ProjectResource project, string version, CancellationToken cancellationToken)
        {
            return Client.Get<ReleaseResource>(project.Link("Releases"), new { version }, cancellationToken);
        }

        public Task<ResourceCollection<ChannelResource>> GetChannels(ProjectResource project)
        {
            return GetChannels(project, CancellationToken.None);
        }

        public Task<ResourceCollection<ChannelResource>> GetChannels(ProjectResource project, CancellationToken cancellationToken)
        {
            return Client.List<ChannelResource>(project.Link("Channels"), cancellationToken);
        }

        public Task<IReadOnlyList<ChannelResource>> GetAllChannels(ProjectResource project)
        {
            return GetAllChannels(project, CancellationToken.None);
        }

        public Task<IReadOnlyList<ChannelResource>> GetAllChannels(ProjectResource project, CancellationToken cancellationToken)
        {
            return Client.ListAll<ChannelResource>(project.Link("Channels"), cancellationToken);
        }

        public Task<ProgressionResource> GetProgression(ProjectResource project)
        {
            return GetProgression(project, CancellationToken.None);
        }

        public Task<ProgressionResource> GetProgression(ProjectResource project, CancellationToken cancellationToken, int? releaseHistoryCount = null)
        {
            var pathParameters = releaseHistoryCount.HasValue ? new { releaseHistoryCount } : null;
            return Client.Get<ProgressionResource>(project.Link("Progression"), pathParameters, cancellationToken);
        }

        public Task<ResourceCollection<ProjectTriggerResource>> GetTriggers(ProjectResource project)
        {
            return GetTriggers(project, CancellationToken.None);
        }

        public Task<ResourceCollection<ProjectTriggerResource>> GetTriggers(ProjectResource project, CancellationToken cancellationToken)
        {
            return Client.List<ProjectTriggerResource>(project.Link("Triggers"), cancellationToken);
        }

        public Task<IReadOnlyList<ProjectTriggerResource>> GetAllTriggers(ProjectResource project)
        {
            return GetAllTriggers(project, CancellationToken.None);
        }

        public Task<IReadOnlyList<ProjectTriggerResource>> GetAllTriggers(ProjectResource project, CancellationToken cancellationToken)
        {
            return Client.ListAll<ProjectTriggerResource>(project.Link("Triggers"), cancellationToken);
        }

        public Task SetLogo(ProjectResource project, string fileName, Stream contents)
        {
            return SetLogo(project, fileName, contents, CancellationToken.None);
        }

        public Task SetLogo(ProjectResource project, string fileName, Stream contents, CancellationToken cancellationToken)
        {
            return Client.Post(project.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false, cancellationToken);
        }

        public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle)
        {
            return CreateOrModify(name, projectGroup, lifecycle, description: null, cloneId: null, CancellationToken.None);
        }

        public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, CancellationToken cancellationToken)
        {
            return CreateOrModify(name, projectGroup, lifecycle, description: null, cloneId: null, cancellationToken);
        }

        public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId = null)
        {
            return CreateOrModify(name, projectGroup, lifecycle, description, cloneId, CancellationToken.None);
        }

        public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, CancellationToken cancellationToken)
        {
            return CreateOrModify(name, projectGroup, lifecycle, description, null, cancellationToken);
        }

        public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId, CancellationToken cancellationToken)
        {
            return new ProjectEditor(this, new ChannelRepository(Repository), new DeploymentProcessRepository(Repository), new ProjectTriggerRepository(Repository), new VariableSetRepository(Repository)).CreateOrModify(name, projectGroup, lifecycle, description, cloneId, cancellationToken);
        }

        public Task<ResourceCollection<RunbookSnapshotResource>> GetRunbookSnapshots(ProjectResource project, int skip = 0, int? take = null, string searchByName = null)
        {
            return GetRunbookSnapshots(project, skip, take, searchByName, CancellationToken.None);
        }

        public Task<ResourceCollection<RunbookSnapshotResource>> GetRunbookSnapshots(ProjectResource project, CancellationToken cancellationToken)
        {
            return GetRunbookSnapshots(project, 0, null, null, cancellationToken);
        }

        public Task<ResourceCollection<RunbookSnapshotResource>> GetRunbookSnapshots(ProjectResource project, int skip, int? take, string searchByName, CancellationToken cancellationToken)
        {
            return Client.List<RunbookSnapshotResource>(project.Link("RunbookSnapshots"), new { skip, take, searchByName }, cancellationToken);
        }

        public Task<IReadOnlyList<RunbookSnapshotResource>> GetAllRunbookSnapshots(ProjectResource project)
        {
            return GetAllRunbookSnapshots(project, CancellationToken.None);
        }

        public Task<IReadOnlyList<RunbookSnapshotResource>> GetAllRunbookSnapshots(ProjectResource project, CancellationToken cancellationToken)
        {
            return Client.ListAll<RunbookSnapshotResource>(project.Link("RunbookSnapshots"), cancellationToken);
        }

        public Task<RunbookSnapshotResource> GetRunbookSnapshotByName(ProjectResource project, string name)
        {
            return GetRunbookSnapshotByName(project, name, CancellationToken.None);
        }

        public Task<RunbookSnapshotResource> GetRunbookSnapshotByName(ProjectResource project, string name, CancellationToken cancellationToken)
        {
            return Client.Get<RunbookSnapshotResource>(project.Link("RunbookSnapshots"), new { name }, cancellationToken);
        }

        public Task<ResourceCollection<RunbookResource>> GetRunbooks(ProjectResource project, int skip = 0, int? take = null, string searchByName = null)
        {
            return GetRunbooks(project, skip, take, searchByName, CancellationToken.None);
        }

        public Task<ResourceCollection<RunbookResource>> GetRunbooks(ProjectResource project, CancellationToken cancellationToken)
        {
            return GetRunbooks(project, 0, null, null, cancellationToken);
        }

        public Task<ResourceCollection<RunbookResource>> GetRunbooks(ProjectResource project, int skip, int? take, string searchByName, CancellationToken cancellationToken)
        {
            return Client.List<RunbookResource>(project.Link("Runbooks"), new { skip, take, searchByName }, cancellationToken);
        }

        public Task<IReadOnlyList<RunbookResource>> GetAllRunbooks(ProjectResource project)
        {
            return GetAllRunbooks(project, CancellationToken.None);
        }

        public Task<IReadOnlyList<RunbookResource>> GetAllRunbooks(ProjectResource project, CancellationToken cancellationToken)
        {
            return Client.ListAll<RunbookResource>(project.Link("Runbooks"), cancellationToken);
        }
    }
}