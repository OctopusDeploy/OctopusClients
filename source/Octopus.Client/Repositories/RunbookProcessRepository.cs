using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IRunbookProcessRepository : IGet<RunbookProcessResource>, IModify<RunbookProcessResource>
    {
        RunbookSnapshotTemplateResource GetTemplate(RunbookProcessResource runbookProcess);
    }
    
    class RunbookProcessRepository : BasicRepository<RunbookProcessResource>, IRunbookProcessRepository
    {
        public RunbookProcessRepository(IOctopusRepository repository)
            : base(repository, "RunbookProcesses")
        {
        }

        public RunbookSnapshotTemplateResource GetTemplate(RunbookProcessResource runbookProcess)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbookProcess.Link("RunbookSnapshotTemplate"));
        }
    }
}