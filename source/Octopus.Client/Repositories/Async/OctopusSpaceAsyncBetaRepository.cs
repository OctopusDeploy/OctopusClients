using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Repositories.Async
{
    public interface IOctopusSpaceAsyncBetaRepository
    {
        IBranchScopedRepository ForBranch(VersionControlBranchResource branch);
    }

    public class OctopusSpaceAsyncBetaRepository : IOctopusSpaceAsyncBetaRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public OctopusSpaceAsyncBetaRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public IBranchScopedRepository ForBranch(VersionControlBranchResource branch)
        {
            return new BranchScopedRepository(repository, branch);
        }
    }
}