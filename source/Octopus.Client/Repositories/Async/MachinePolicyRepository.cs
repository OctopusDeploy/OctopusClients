using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IMachinePolicyRepository : IFindByName<MachinePolicyResource>, ICreate<MachinePolicyResource>, IModify<MachinePolicyResource>, IGet<MachinePolicyResource>, IDelete<MachinePolicyResource>
    {
    }

    class MachinePolicyRepository : BasicRepository<MachinePolicyResource>, IMachinePolicyRepository
    {
        public MachinePolicyRepository(IOctopusAsyncClient client) : base(client, "MachinePolicies")
        {
        }
    }
}
