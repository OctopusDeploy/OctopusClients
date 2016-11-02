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
        readonly IOctopusAsyncClient client;

        public MachineRoleRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public async Task<List<string>> GetAllRoleNames()
        {
            var result = await client.Get<string[]>(client.RootDocument.Link("MachineRoles")).ConfigureAwait(false);
            return result.ToList();
        }
    }
}
