using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IBranchScopedRepository
    {
        IVcsRunbookRepository Runbooks { get; }
    }

    public class BranchScopedRepository : IBranchScopedRepository
    {
        private readonly IOctopusRepository repository;

        public BranchScopedRepository(IOctopusRepository repository, VersionControlBranchResource branch)
        {
            this.repository = repository;
            Runbooks = new VcsRunbookRepository(repository, branch);
        }
        
        public IVcsRunbookRepository Runbooks { get; }
    }
}