using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IBranchScopedRepository
    {
        IVcsRunbookRepository Runbooks { get; }
    }

    public class BranchScopedRepository : IBranchScopedRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public BranchScopedRepository(IOctopusAsyncRepository repository, VersionControlBranchResource branch)
        {
            this.repository = repository;
            Runbooks = new VcsRunbookRepository(repository, branch);
        }
        
        public IVcsRunbookRepository Runbooks { get; }
    }
}