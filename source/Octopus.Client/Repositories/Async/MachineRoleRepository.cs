using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface IMachineRoleRepository
    {
        Task<List<string>> GetAllRoleNames();
    }

    class MachineRoleRepository : IMachineRoleRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public MachineRoleRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<string>> GetAllRoleNames()
        {
            var result = await repository.Client.Get<string[]>(repository.Link("MachineRoles")).ConfigureAwait(false);
            return result.ToList();
        }
    }
}
