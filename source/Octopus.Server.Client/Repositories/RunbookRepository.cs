using System.Linq;
using Octopus.Client.Editors;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories
{
    public interface IRunbookRepository : IFindByName<RunbookResource>, IGet<RunbookResource>, ICreate<RunbookResource>, IModify<RunbookResource>, IDelete<RunbookResource>
    {
        RunbookResource FindByName(ProjectResource project, string name);
        RunbookEditor CreateOrModify(ProjectResource project, string name, string description);
        RunbookSnapshotTemplateResource GetRunbookSnapshotTemplate(RunbookResource runbook);
        RunbookRunTemplateResource GetRunbookRunTemplate(RunbookResource runbook);
        RunbookRunPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget);
        RunbookRunResource Run(RunbookResource runbook, RunbookRunResource runbookRun);
        RunbookRunResource[] Run(RunbookResource runbook, RunbookRunParameters runbookRunParameters);

        // Config as Code methods
        /// <summary>
        /// Returns a Runbook for a specific Git ref and slug
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        RunbookResource Get(ProjectResource project, string gitRef, string slug);

        /// <summary>
        /// Creates a new Runbook with an empty process for a specific Git ref and slug
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        RunbookResource Create(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage);

        /// <summary>
        /// Modifies a Runbook for a specific Git ref and slug
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        RunbookResource Modify(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage);

        /// <summary>
        /// Deletes a Runbook for a specific Git ref and slug
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        void Delete(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage);

        /// <summary>
        /// Runs a Runbook for a specific Git ref and slug. Multiple runs for different environments and tenant combinations are configured in the run parameters.
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        RunbookRunGitResource Run(ProjectResource project, string gitRef, string slug, RunGitRunbookParameters runbookRunParameters);

        /// <summary>
        /// Get a Runbook snapshot template for a specific Git ref and slug
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        RunbookSnapshotTemplateResource GetRunbookSnapshotTemplate(ProjectResource project, string gitRef, string slug);

        /// <summary>
        /// Get a Runbook run template for a specific Git ref and slug
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        RunbookRunTemplateResource GetRunbookRunTemplate(ProjectResource project, string gitRef, string slug);

        /// <summary>
        /// Get a Runbook run preview for a specific Git ref and slug. Multple previews for different environments and tenant combinations are configured in the run parameters.
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        RunbookRunPreviewResource[] GetPreview(ProjectResource project, string gitRef, string slug, RunbookRunPreviewParameters runbookRunParameters);
    }

    class RunbookRepository : BasicRepository<RunbookResource>, IRunbookRepository
    {
        private readonly string baseGitUri = "~/api/{spaceId}/projects/{projectId}/{gitRef}";
        private readonly SemanticVersion versionAfterWhichRunbookRunParametersAreAvailable;
        private readonly SemanticVersion integrationTestVersion;

        public RunbookRepository(IOctopusRepository repository)
            : base(repository, "Runbooks")
        {
            integrationTestVersion = SemanticVersion.Parse("0.0.0-local");
            versionAfterWhichRunbookRunParametersAreAvailable = SemanticVersion.Parse("2020.3.1");
        }

        public RunbookResource FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Runbooks"));
        }

        public RunbookEditor CreateOrModify(ProjectResource project, string name, string description)
        {
            return new RunbookEditor(this, new RunbookProcessRepository(Repository)).CreateOrModify(project, name, description);
        }

        public RunbookSnapshotTemplateResource GetRunbookSnapshotTemplate(RunbookResource runbook)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbook.Link("RunbookSnapshotTemplate"));
        }

        public RunbookRunTemplateResource GetRunbookRunTemplate(RunbookResource runbook)
        {
            return Client.Get<RunbookRunTemplateResource>(runbook.Link("RunbookRunTemplate"));
        }

        public RunbookRunPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget)
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

        public RunbookRunResource Run(RunbookResource runbook, RunbookRunResource runbookRun)
        {
            var serverSupportsRunbookRunParameters = ServerSupportsRunbookRunParameters(Repository.LoadRootDocument().Version);

            return serverSupportsRunbookRunParameters
                ? Run(runbook, RunbookRunParameters.MapFrom(runbookRun)).FirstOrDefault()
                : Client.Post<object, RunbookRunResource>(runbook.Link("CreateRunbookRun"), runbookRun);
        }

        public RunbookRunResource[] Run(RunbookResource runbook, RunbookRunParameters runbookRunParameters)
        {
            var serverVersion = Repository.LoadRootDocument().Version;
            var serverSupportsRunbookRunParameters = ServerSupportsRunbookRunParameters(serverVersion);

            if (serverSupportsRunbookRunParameters == false)
                throw new UnsupportedApiVersionException($"This Octopus Deploy server is an older version ({serverVersion}) that does not yet support RunbookRunParameters. " +
                                                         "Please update your Octopus Deploy server to 2020.3.* or newer to access this feature.");

            return Client.Post<object, RunbookRunResource[]>(runbook.Link("CreateRunbookRun"), runbookRunParameters);
        }

        // Config as Code
        public RunbookResource Get(ProjectResource project, string gitRef, string slug)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}";

            return Client.Get<RunbookResource>(
                route,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = slug
                }
            );
        }

        public RunbookResource Create(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage)
        {
            var route = $"{baseGitUri}/runbooks/v2";
            var command = AppendCommitMessage(runbook, commitMessage);

            return Client.Create<ModifyRunbookCommand, RunbookResource>(
                route,
                command,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef
                }
            );
        }

        public RunbookResource Modify(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}";
            var command = AppendCommitMessage(runbook, commitMessage);

            return Client.Update<ModifyRunbookCommand, RunbookResource>(
                route,
                command,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = runbook.Id
                }
            );
        }

        public void Delete(ProjectResource project, string gitRef, RunbookResource runbook, string commitMessage)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}";

            Client.Delete(
                route,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = runbook.Id
                },
                new
                {
                    ChangeDescription = commitMessage
                }
            );
        }

        public RunbookRunGitResource Run(ProjectResource project, string gitRef, string slug, RunGitRunbookParameters runbookRunParameters)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}/run/v1";

            return Client.Post<RunGitRunbookParameters, RunbookRunGitResource>(
                route,
                runbookRunParameters,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = slug
                }
            );
        }

        public RunbookSnapshotTemplateResource GetRunbookSnapshotTemplate(ProjectResource project, string gitRef, string slug)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}/runbookSnapshotTemplate";

            return Client.Get<RunbookSnapshotTemplateResource>(
                route,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = slug
                }
            );
        }

        public RunbookRunTemplateResource GetRunbookRunTemplate(ProjectResource project, string gitRef, string slug)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}/runbookRunTemplate";

            return Client.Get<RunbookRunTemplateResource>(
                route,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = slug
                }
            );
        }

        public RunbookRunPreviewResource[] GetPreview(ProjectResource project, string gitRef, string slug, RunbookRunPreviewParameters runbookRunPreviewParameters)
        {
            var route = $"{baseGitUri}/runbooks/{{id}}/runbookRuns/previews";

            return Client.Post<RunbookRunPreviewParameters, RunbookRunPreviewResource[]>(
                route,
                runbookRunPreviewParameters,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = slug
                }
            );
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