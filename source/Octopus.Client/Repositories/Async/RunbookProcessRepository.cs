using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IRunbookProcessRepository : IGet<RunbookProcessResource>, IModify<RunbookProcessResource>
    {
        Task<RunbookSnapshotTemplateResource> GetTemplate(RunbookProcessResource runbookProcess);
    }

    class RunbookProcessRepository : BasicRepository<RunbookProcessResource>, IRunbookProcessRepository
    {
        public RunbookProcessRepository(IOctopusAsyncRepository repository)
            : base(repository, "RunbookProcess")
        {
        }

        public Task<RunbookSnapshotTemplateResource> GetTemplate(RunbookProcessResource runbookProcess)
        {
            return Client.Get<RunbookSnapshotTemplateResource>(runbookProcess.Link("RunbookSnapshotTemplate"));
        }
    }
}
