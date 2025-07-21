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
        public MachinePolicyRepository(IOctopusAsyncRepository repository) : base(repository, "MachinePolicies")
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

        public async Task<MachinePolicyResource> GetTemplate()
        {
            var link = await Repository.Link("MachinePolicyTemplate").ConfigureAwait(false);
            return await Client.Get<MachinePolicyResource>(link).ConfigureAwait(false);
        }
    }
}
