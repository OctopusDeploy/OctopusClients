using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IOctopusSpaceBetaRepository
    {
        IBranchScopedRepository ForBranch(VersionControlBranchResource branch);
    }

    public class OctopusSpaceBetaRepository : IOctopusSpaceBetaRepository
    {
        private readonly IOctopusRepository repository;

        public OctopusSpaceBetaRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public IBranchScopedRepository ForBranch(VersionControlBranchResource branch)
        {
            return new BranchScopedRepository(repository, branch);
        }
    }
}