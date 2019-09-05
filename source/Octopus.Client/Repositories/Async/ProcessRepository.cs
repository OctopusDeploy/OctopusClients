using Octopus.Client.Model.Processes;

namespace Octopus.Client.Repositories.Async
{
    public interface IProcessRepository : IFindByName<ProcessResource>, IGet<ProcessResource>, IModify<ProcessResource>
    {
    }

    class ProcessRepository : BasicRepository<ProcessResource>, IProcessRepository
    {
        public ProcessRepository(IOctopusAsyncRepository repository)
            : base(repository, "Processes")
        {
        }
    }
}
