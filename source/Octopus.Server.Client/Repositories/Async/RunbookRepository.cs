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
        Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description);
        Task<RunbookSnapshotTemplateResource> GetRunbookSnapshotTemplate(RunbookResource runbook);
        Task<RunbookRunTemplateResource> GetRunbookRunTemplate(RunbookResource runbook);
        Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget);
        Task<RunbookRunResource> Run(RunbookResource runbook, RunbookRunResource runbookRun);
        Task<RunbookRunResource[]> Run(RunbookResource runbook, RunbookRunParameters runbookRunParameters);

        // Config as Code methods
        Task<RunbookResource> Get(ProjectResource project, string gitRef, string slug, CancellationToken cancellationToken);
        Task<RunbookResource> Create(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage, CancellationToken cancellationToken);
        Task<RunbookResource> Modify(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage, CancellationToken cancellationToken);
        Task Delete(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage, CancellationToken cancellationToken);
        Task<RunbookRunGitResource> Run(ProjectResource project, string gitRef, string slug, RunGitRunbookParameters runbookRunParameters, CancellationToken cancellationToken);
        Task<RunbookSnapshotTemplateResource> GetRunbookSnapshotTemplate(ProjectResource project, string gitRef, string slug, CancellationToken cancellationToken);
        Task<RunbookRunTemplateResource> GetRunbookRunTemplate(ProjectResource project, string gitRef, string slug, CancellationToken cancellationToken);
        Task<RunbookRunPreviewResource> GetPreview(ProjectResource project, string gitRef, string slug, RunbookRunPreviewParameters runbookRunParameters, CancellationToken cancellationToken);
    }

    class RunbookRepository : BasicRepository<RunbookResource>, IRunbookRepository
    {
        private readonly string baseGitUri = "~/api/{spaceId}/projects/{projectId}/{gitRef}";
        private readonly SemanticVersion integrationTestVersion;
        private readonly SemanticVersion versionAfterWhichRunbookRunParametersAreAvailable;

        public RunbookRepository(IOctopusAsyncRepository repository)
            : base(repository, "Runbooks")
        {
            integrationTestVersion = SemanticVersion.Parse("0.0.0-local");
            versionAfterWhichRunbookRunParametersAreAvailable = SemanticVersion.Parse("2020.3.1");
        }

        public Task<RunbookResource> FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Runbooks"));
        }

        public Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description)
        {
            return new RunbookEditor(this, new RunbookProcessRepository(Repository)).CreateOrModify(project, name, description);
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

        // Config as Code
        public async Task<RunbookResource> Get(ProjectResource project, string gitRef, string slug, CancellationToken cancellationToken)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}";

            return await Client.Get<RunbookResource>(
                route,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = slug
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<RunbookResource> Create(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage, CancellationToken cancellationToken)
        {
            var route = $"{baseGitUri}/runbooks/v2";
            var command = AppendCommitMessage(runbook, commitMessage);

            return await Client.Create<ModifyRunbookCommand, RunbookResource>(
                route,
                command,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<RunbookResource> Modify(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage, CancellationToken cancellationToken)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}";
            var command = AppendCommitMessage(runbook, commitMessage);

            return await Client.Update<ModifyRunbookCommand, RunbookResource>(
                route,
                command,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = runbook.Id
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task Delete(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage, CancellationToken cancellationToken)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}";

            await Client.Delete(
                route,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    slug = runbook.Id
                },
                new
                {
                    ChangeDescription = commitMessage
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<RunbookRunGitResource> Run(ProjectResource project, string gitRef, string slug, RunGitRunbookParameters runbookRunParameters, CancellationToken cancellationToken)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}/run/v1";

            return await Client.Post<RunGitRunbookParameters, RunbookRunGitResource>(
                route,
                runbookRunParameters,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = slug
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<RunbookSnapshotTemplateResource> GetRunbookSnapshotTemplate(ProjectResource project, string gitRef, string slug, CancellationToken cancellationToken)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}/runbookSnapshotTemplate";

            return await Client.Get<RunbookSnapshotTemplateResource>(
                route,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = slug
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<RunbookRunTemplateResource> GetRunbookRunTemplate(ProjectResource project, string gitRef, string slug, CancellationToken cancellationToken)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}/runbookRunTemplate";

            return await Client.Get<RunbookRunTemplateResource>(
                route,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = slug
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<RunbookRunPreviewResource> GetPreview(ProjectResource project, string gitRef, string slug, RunbookRunPreviewParameters runbookRunPreviewParameters, CancellationToken cancellationToken)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}/runbookRuns/previews";

            return await Client.Post<RunbookRunPreviewParameters, RunbookRunPreviewResource>(
                route,
                runbookRunPreviewParameters,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = slug
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        ModifyRunbookCommand AppendCommitMessage(RunbookResource runbook, string commitMessage)
        {
            var json = Serializer.Serialize(runbook);
            var command = Serializer.Deserialize<ModifyRunbookCommand>(json);

            command.ChangeDescription = commitMessage;
            return command;
        }
    }
}
