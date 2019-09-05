using Octopus.Client.Model.Processes;

namespace Octopus.Client.Repositories
{
    public interface IProcessRepository : IFindByName<ProcessResource>, IGet<ProcessResource>, IModify<ProcessResource>
    {
    }
    
    class ProcessRepository : BasicRepository<ProcessResource>, IProcessRepository
    {
        public ProcessRepository(IOctopusRepository repository)
            : base(repository, "Processes")
        {
        }
    }
}