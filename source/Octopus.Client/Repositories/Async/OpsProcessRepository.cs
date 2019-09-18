using Octopus.Client.Model.OpsProcesses;

namespace Octopus.Client.Repositories.Async
{
    public interface IOpsProcessRepository : IFindByName<OpsProcessResource>, IGet<OpsProcessResource>, IModify<OpsProcessResource>
    {
    }

    class OpsProcessRepository : BasicRepository<OpsProcessResource>, IOpsProcessRepository
    {
        public OpsProcessRepository(IOctopusAsyncRepository repository)
            : base(repository, "OpsProcesses")
        {
        }
    }
}
