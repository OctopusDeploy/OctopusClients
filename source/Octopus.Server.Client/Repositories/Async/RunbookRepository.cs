using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories.Async
{
    public interface IRunbookRepository : IFindByName<RunbookResource>, IGet<RunbookResource>, ICreate<RunbookResource>, IModify<RunbookResource>, IDelete<RunbookResource>
    {
        Task<RunbookResource> FindByName(ProjectResource project, string name);
        Task<RunbookResource> GetInGit(string id, ProjectResource project, string gitRef, CancellationToken cancellationToken);
        Task<RunbookResource> CreateInGit(RunbookResource resource, string gitRef, string commitMessage, CancellationToken cancellationToken);
        Task<RunbookResource> ModifyInGit(RunbookResource resource, string gitRef, string commitMessage, CancellationToken cancellationToken);
        Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description);
        Task<RunbookEditor> CreateOrModifyInGit(ProjectResource project, string slug, string name, string description, string gitRef, string commitMessage, CancellationToken cancellationToken);
        Task<RunbookSnapshotTemplateResource> GetRunbookSnapshotTemplate(RunbookResource runbook);
        Task<RunbookRunTemplateResource> GetRunbookRunTemplate(RunbookResource runbook);
        Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget);
        Task<RunbookRunResource> Run(RunbookResource runbook, RunbookRunResource runbookRun);
        Task<RunbookRunResource[]> Run(RunbookResource runbook, RunbookRunParameters runbookRunParameters);
        
        Task<RunbookRunResource[]> RunInGit(RunbookResource runbook, RunbookRunGitParameters runbookRunParameters, string gitRef, CancellationToken cancellationToken);
    }

    class RunbookRepository : BasicRepository<RunbookResource>, IRunbookRepository
    {
        private readonly SemanticVersion integrationTestVersion;
        private readonly SemanticVersion versionAfterWhichRunbookRunParametersAreAvailable;

        public RunbookRepository(IOctopusAsyncRepository repository)
            : base(repository, "Runbooks")
        {
            integrationTestVersion = SemanticVersion.Parse("0.0.0-local");
            versionAfterWhichRunbookRunParametersAreAvailable = SemanticVersion.Parse("2020.3.1");
        }

        public async Task<RunbookResource> GetInGit(string id, ProjectResource project, string gitRef, CancellationToken cancellationToken)
        {
            return await GetInGit(id, project.SpaceId, project.Id, gitRef, cancellationToken);
        }
        
        async Task<RunbookResource> GetInGit(string id, string spaceId, string projectId, string gitRef, CancellationToken cancellationToken)
            => await Client.Get<RunbookResource>($"/api/{spaceId}/projects/{projectId}/{gitRef}/runbooks/{id}", cancellationToken);
        
        public async Task<RunbookResource> CreateInGit(RunbookResource resource, string gitRef, string commitMessage, CancellationToken cancellationToken)
        {
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyRunbookCommand>(json);
            command.ChangeDescription = commitMessage;
            
            await Client.Post<RunbookResource>($"/api/{resource.SpaceId}/projects/{resource.ProjectId}/{gitRef}/runbooks/", command, cancellationToken);
            return await GetInGit(resource.Id, resource.SpaceId, resource.ProjectId, gitRef, cancellationToken);
        }
        
        public async Task<RunbookResource> ModifyInGit(RunbookResource resource, string gitRef, string commitMessage, CancellationToken cancellationToken)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyRunbookCommand>(json);
            command.ChangeDescription = commitMessage;
            
            await Client.Put<RunbookResource>($"/api/{resource.SpaceId}/projects/{resource.ProjectId}/{gitRef}/runbooks/{resource.Id}", command, cancellationToken);
            return await GetInGit(resource.Id, resource.SpaceId, resource.ProjectId, gitRef, cancellationToken);
        }
        
        public async Task DeleteInGit(RunbookResource resource, string gitRef, string commitMessage)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<DeleteRunbookCommand>(json);
            command.ChangeDescription = commitMessage;
            
            await Client.Delete($"/api/{resource.SpaceId}/projects/{resource.ProjectId}/{gitRef}/runbooks/{resource.Id}", command);
        }
        
        public Task<RunbookResource> FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Runbooks"));
        }

        public Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description)
        {
            return new RunbookEditor(this, new RunbookProcessRepository(Repository)).CreateOrModify(project, name, description);
        }

        public Task<RunbookEditor> CreateOrModifyInGit(ProjectResource project, string slug, string name, string description, string gitRef, string commitMessage, CancellationToken cancellationToken)
        {
            return new RunbookEditor(this, new RunbookProcessRepository(Repository)).CreateOrModifyInGit(project, slug, name, description, gitRef,  commitMessage, cancellationToken);
        }
        
        public Task<RunbookSnapshotTemplateResource> GetRunbookSnapshotTemplate(RunbookResource runbook)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbook.Link("RunbookSnapshotTemplate"));
        }

        public Task<RunbookRunTemplateResource> GetRunbookRunTemplate(RunbookResource runbook)
        {
            return Client.Get<RunbookRunTemplateResource>(runbook.Link("RunbookRunTemplate"));
        }

        public Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return Client.Get<RunbookRunPreviewResource>(promotionTarget.Link("RunbookRunPreview"));
        }
        
        private bool ServerSupportsRunbookRunParameters(string version)
        {
            var serverVersion = SemanticVersion.Parse(version);

            // Note: We want to ensure the server version is >= *any* 2020.3.1, including all pre-releases to consider what may be rolled out to Octopus Cloud.
            var preReleaseAgnosticServerVersion =
                new SemanticVersion(serverVersion.Major, serverVersion.Minor, serverVersion.Patch);

            return preReleaseAgnosticServerVersion >= versionAfterWhichRunbookRunParametersAreAvailable ||
                   serverVersion == integrationTestVersion;
        }

        public async Task<RunbookRunResource> Run(RunbookResource runbook, RunbookRunResource runbookRun)
        {
            var serverSupportsRunbookRunParameters = ServerSupportsRunbookRunParameters((await Repository.LoadRootDocument()).Version);

            return serverSupportsRunbookRunParameters
                ? (await Run(runbook, RunbookRunParameters.MapFrom(runbookRun))).FirstOrDefault()
                : await Client.Post<object, RunbookRunResource>(runbook.Link("CreateRunbookRun"), runbookRun);
        }


        public async Task<RunbookRunResource[]> Run(RunbookResource runbook, RunbookRunParameters runbookRunParameters)
        {
            var serverVersion = (await Repository.LoadRootDocument()).Version;
            var serverSupportsRunbookRunParameters = ServerSupportsRunbookRunParameters(serverVersion);

            if (serverSupportsRunbookRunParameters == false)
                throw new UnsupportedApiVersionException($"This Octopus Deploy server is an older version ({serverVersion}) that does not yet support RunbookRunParameters. " +
                                                         $"Please update your Octopus Deploy server to 2020.3.* or newer to access this feature.");

            return await Client.Post<object, RunbookRunResource[]>(runbook.Link("CreateRunbookRun"), runbookRunParameters);
        }
       
        public async Task<RunbookRunResource[]> RunInGit(RunbookResource runbook, RunbookRunGitParameters runbookRunParameters, string gitRef, CancellationToken cancellationToken)
        {
            var serverVersion = (await Repository.LoadRootDocument()).Version;
            var serverSupportsRunbookRunParameters = ServerSupportsRunbookRunParameters(serverVersion);

            if (serverSupportsRunbookRunParameters == false)
                throw new UnsupportedApiVersionException($"This Octopus Deploy server is an older version ({serverVersion}) that does not yet support RunbookRunParameters. " +
                                                         $"Please update your Octopus Deploy server to 2020.3.* or newer to access this feature.");
            
            return await Client.Post<object, RunbookRunResource[]>($"/api/{runbook.SpaceId}/projects/{runbook.ProjectId}/{gitRef}/runbooks/{runbook.Id}/run/v1", runbookRunParameters);
        }
    }
}
