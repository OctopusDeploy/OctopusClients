using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IBranchScopedRepository
    {
        IVcsRunbookRepository VcsRunbooks { get; }
    }

    public class BranchScopedRepository : IBranchScopedRepository
    {
        private readonly IOctopusRepository repository;

        public BranchScopedRepository(IOctopusRepository repository, VersionControlBranchResource branch)
        {
            this.repository = repository;
            VcsRunbooks = new VcsRunbookRepository(repository, branch);
        }
        
        public IVcsRunbookRepository VcsRunbooks { get; }
    }
}