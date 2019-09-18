using Octopus.Client.Model.OpsProcesses;

namespace Octopus.Client.Repositories
{
    public interface IOpsProcessRepository : IFindByName<OpsProcessResource>, IGet<OpsProcessResource>, IModify<OpsProcessResource>
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