using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IRunbookRepository : IFindByName<RunbookResource>, IGet<RunbookResource>, ICreate<RunbookResource>, IModify<RunbookResource>, IDelete<RunbookResource>
    {
    }
    
    class RunbookRepository : BasicRepository<RunbookResource>, IRunbookRepository
    {
        public RunbookRepository(IOctopusRepository repository)
            : base(repository, "Runbooks")
        {
        }
    }
}