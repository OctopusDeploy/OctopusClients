using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IMachinePolicyRepository : IFindByName<MachinePolicyResource>, ICreate<MachinePolicyResource>, IModify<MachinePolicyResource>, IGet<MachinePolicyResource>, IDelete<MachinePolicyResource>
    {
        Task<List<MachineResource>> GetMachines(MachinePolicyResource machinePolicy);
        Task<MachinePolicyResource> GetTemplate();

    }

    class MachinePolicyRepository : BasicRepository<MachinePolicyResource>, IMachinePolicyRepository
    {
        public MachinePolicyRepository(IOctopusAsyncClient client) : base(client, "MachinePolicies")
        {
        }

        public async Task<List<MachineResource>> GetMachines(MachinePolicyResource machinePolicy)
        {
            var resources = new List<MachineResource>();

            await Client.Paginate<MachineResource>(machinePolicy.Link("Machines"), new { }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            }).ConfigureAwait(false);

            return resources;
        }

        public Task<MachinePolicyResource> GetTemplate() 
            => Client.Get<MachinePolicyResource>(Client.RootDocument.Link("MachinePolicyTemplate"));
    }
}
