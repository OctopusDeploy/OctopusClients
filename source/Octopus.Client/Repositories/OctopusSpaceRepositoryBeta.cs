using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IOctopusSpaceRepositoryBeta
    {
        IBranchScopedRepository ForBranch(VersionControlBranchResource branch);
    }

    public class OctopusSpaceRepositoryBeta : IOctopusSpaceRepositoryBeta
    {
        private readonly IOctopusRepository repository;

        public OctopusSpaceRepositoryBeta(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public IBranchScopedRepository ForBranch(VersionControlBranchResource branch)
        {
            return new BranchScopedRepository(repository, branch);
        }
    }
}