using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories
{
    public interface IRunbookProcessRepository : IGet<RunbookProcessResource>, IModify<RunbookProcessResource>
    {
        RunbookSnapshotTemplateResource GetTemplate(RunbookProcessResource runbookProcess);

        /// <summary>
        /// Get a Runbook process for a specific Git ref and slug
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        RunbookProcessResource Get(ProjectResource project, string gitRef, string slug);

        /// <summary>
        /// Modifies a Runbook process for a specific Git ref and slug
        /// </summary>
        /// <remarks>This operation is for Config as Code Runbooks only</remarks>
        RunbookProcessResource Modify(ProjectResource project, string gitRef, RunbookProcessResource runbookProcess, string commitMessage);
    }
    
    class RunbookProcessRepository : BasicRepository<RunbookProcessResource>, IRunbookProcessRepository
    {
        private readonly string baseGitUri = "~/api/{spaceId}/projects/{projectId}/{gitRef}";

        public RunbookProcessRepository(IOctopusRepository repository)
            : base(repository, "RunbookProcesses")
        {
        }

        public RunbookSnapshotTemplateResource GetTemplate(RunbookProcessResource runbookProcess)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbookProcess.Link("RunbookSnapshotTemplate"));
        }

        public RunbookProcessResource Get(ProjectResource project, string gitRef, string slug)
        {
            var route = $"{baseGitUri}/runbookProcesses/{{id}}";

            return Client.Get<RunbookProcessResource>(
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

        public RunbookProcessResource Modify(ProjectResource project, string gitRef, RunbookProcessResource runbookProcess, string commitMessage)
        {
            var route = $"{baseGitUri}/runbookProcesses/{{id}}";

            var json = Serializer.Serialize(runbookProcess);
            var command = Serializer.Deserialize<ModifyRunbookProcessCommand>(json);
            command.ChangeDescription = commitMessage;

            return Client.Update<ModifyRunbookProcessCommand, RunbookProcessResource>(
                route,
                command,
                new
                {
                    spaceId = project.SpaceId,
                    projectId = project.Id,
                    gitRef = gitRef,
                    id = runbookProcess.Id
                }
            );
        }
    }
}