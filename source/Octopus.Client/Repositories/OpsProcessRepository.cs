using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IOpsProcessRepository : IFindByName<OpsProcessResource>, IGet<OpsProcessResource>, ICreate<OpsProcessResource>, IModify<OpsProcessResource>, IDelete<OpsProcessResource>
    {
    }
    
    class OpsProcessRepository : BasicRepository<OpsProcessResource>, IOpsProcessRepository
    {
        public OpsProcessRepository(IOctopusRepository repository)
            : base(repository, "OpsProcesses")
        {
        }
    }
}