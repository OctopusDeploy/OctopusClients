using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IMachinePolicyRepository : IFindByName<MachinePolicyResource>, ICreate<MachinePolicyResource>, IModify<MachinePolicyResource>, IGet<MachinePolicyResource>, IDelete<MachinePolicyResource>
    {
        List<MachineResource> GetMachines(MachinePolicyResource machinePolicy);
        MachinePolicyResource GetTemplate();
    }
    
    class MachinePolicyRepository : BasicRepository<MachinePolicyResource>, IMachinePolicyRepository
    {
        public MachinePolicyRepository(IOctopusClient client) : base(client, "MachinePolicies")
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

        public MachinePolicyResource GetTemplate()
             => Client.Get<MachinePolicyResource>(Client.RootDocument.Link("MachinePolicyTemplate"));
    }
}