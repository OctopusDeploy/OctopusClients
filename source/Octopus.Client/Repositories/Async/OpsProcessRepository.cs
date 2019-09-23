using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IOpsProcessRepository : IFindByName<OpsProcessResource>, IGet<OpsProcessResource>, ICreate<OpsProcessResource>, IModify<OpsProcessResource>, IDelete<OpsProcessResource>
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
