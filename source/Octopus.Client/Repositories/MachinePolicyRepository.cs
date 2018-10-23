using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IMachinePolicyRepository : IFindByName<MachinePolicyResource>, ICreate<MachinePolicyResource>, IModify<MachinePolicyResource>, IGet<MachinePolicyResource>, IDelete<MachinePolicyResource>
    {
        List<MachineResource> GetMachines(MachinePolicyResource machinePolicy);
    }
    
    class MachinePolicyRepository : BasicRepository<MachinePolicyResource>, IMachinePolicyRepository
    {
        public MachinePolicyRepository(IOctopusRepository repository) : base(repository, "MachinePolicies")
        {
        }

        public List<MachineResource> GetMachines(MachinePolicyResource machinePolicy)
        {
            var resources = new List<MachineResource>();

            Client.Paginate<MachineResource>(machinePolicy.Link("Machines"), new { }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            });

            return resources;
        }
    }
}