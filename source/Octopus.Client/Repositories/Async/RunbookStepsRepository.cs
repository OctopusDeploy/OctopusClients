using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IRunbookStepsRepository : IGet<RunbookStepsResource>, IModify<RunbookStepsResource>
    {
        Task<RunbookSnapshotTemplateResource> GetTemplate(RunbookStepsResource runbookSteps);
    }

    class RunbookStepsRepository : BasicRepository<RunbookStepsResource>, IRunbookStepsRepository
    {
        public RunbookStepsRepository(IOctopusAsyncRepository repository)
            : base(repository, "RunbookSteps")
        {
        }

        public Task<RunbookSnapshotTemplateResource> GetTemplate(RunbookStepsResource runbookSteps)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbookSteps.Link("RunbookSnapshotTemplate"));
        }
    }
}
