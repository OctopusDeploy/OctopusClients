using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IBranchScopedRepository
    {
        IVcsRunbookRepository VcsRunbooks { get; }
    }

    public class BranchScopedRepository : IBranchScopedRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public BranchScopedRepository(IOctopusAsyncRepository repository, VersionControlBranchResource branch)
        {
            this.repository = repository;
            VcsRunbooks = new VcsRunbookRepository(repository, branch);
        }
        
        public IVcsRunbookRepository VcsRunbooks { get; }
    }
}