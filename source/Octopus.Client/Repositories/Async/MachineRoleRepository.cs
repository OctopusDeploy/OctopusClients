using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface IMachineRoleRepository
    {
        Task<IReadOnlyList<string>> GetAllRoleNames();
    }

    class MachineRoleRepository : IMachineRoleRepository
    {
        readonly IOctopusAsyncClient client;

        public MachineRoleRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public async Task<IReadOnlyList<string>> GetAllRoleNames()
        {
            return await client.Get<string[]>(client.RootDocument.Link("MachineRoles")).ConfigureAwait(false);
        }
    }
}
