using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IRunbookStepsRepository : IGet<RunbookStepsResource>, IModify<RunbookStepsResource>
    {
        RunbookSnapshotTemplateResource GetTemplate(RunbookStepsResource runbookSteps);
    }
    
    class RunbookStepsRepository : BasicRepository<RunbookStepsResource>, IRunbookStepsRepository
    {
        public RunbookStepsRepository(IOctopusRepository repository)
            : base(repository, "RunbookSteps")
        {
        }

        public RunbookSnapshotTemplateResource GetTemplate(RunbookStepsResource runbookSteps)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbookSteps.Link("RunbookSnapshotTemplate"));
        }
    }
}