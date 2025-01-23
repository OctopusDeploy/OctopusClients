using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories.Async
{
    public interface IRunbookProcessRepository : IGet<RunbookProcessResource>, IModify<RunbookProcessResource>
    {
        Task<RunbookSnapshotTemplateResource> GetTemplate(RunbookProcessResource runbookProcess);

        // Config as Code methods

        /// <summary>
        /// Get a Runbook process for a specific Git ref and slug
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        Task<RunbookProcessResource> Get(ProjectResource project, string gitRef, string slug, CancellationToken cancellationToken);

        /// <summary>
        /// Modifies a Runbook process for a specific Git ref and slug
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        Task<RunbookProcessResource> Modify(ProjectResource project, string gitRef, RunbookProcessResource runbookProcess, string commitMessage, CancellationToken cancellationToken);
    }

    class RunbookProcessRepository : BasicRepository<RunbookProcessResource>, IRunbookProcessRepository
    {
        private readonly string baseGitUri = "~/api/{spaceId}/projects/{projectId}/{gitRef}";

        public RunbookProcessRepository(IOctopusAsyncRepository repository)
            : base(repository, "RunbookProcesses")
        {
        }

        public Task<RunbookSnapshotTemplateResource> GetTemplate(RunbookProcessResource runbookProcess)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbookProcess.Link("RunbookSnapshotTemplate"));
        }

        public async Task<RunbookProcessResource> Get(ProjectResource project, string gitRef, string slug, CancellationToken cancellationToken)
        {
            var route = $"{baseGitUri}/runbookProcesses/{{id}}";

            return await Client.Get<RunbookProcessResource>(
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

        public async Task<RunbookProcessResource> Modify(ProjectResource project, string gitRef, RunbookProcessResource runbookProcess, string commitMessage, CancellationToken cancellationToken)
        {
            var route = $"{baseGitUri}/runbookProcesses/{{id}}";

            var json = Serializer.Serialize(runbookProcess);
            var command = Serializer.Deserialize<ModifyRunbookProcessCommand>(json);
            command.ChangeDescription = commitMessage;

            return await Client.Update<ModifyRunbookProcessCommand, RunbookProcessResource>(
                route,
                command,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = runbookProcess.Id
                },
                cancellationToken
            ).ConfigureAwait(false);
        }
    }
}
