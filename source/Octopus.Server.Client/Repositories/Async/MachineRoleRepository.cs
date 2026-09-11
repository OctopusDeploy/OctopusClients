using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface IMachineRoleRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<string>> GetAllRoleNames();
        Task<List<string>> GetAllRoleNames(CancellationToken cancellationToken);
    }

    class MachineRoleRepository : IMachineRoleRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public MachineRoleRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<string>> GetAllRoleNames()
            => GetAllRoleNames(CancellationToken.None);

        public async Task<List<string>> GetAllRoleNames(CancellationToken cancellationToken)
        {
            var result = await repository.Client.Get<string[]>(await repository.Link("MachineRoles").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
            return result.ToList();
        }
    }
}
