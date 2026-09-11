using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IMachinePolicyRepository : IFindByName<MachinePolicyResource>, ICreate<MachinePolicyResource>, IModify<MachinePolicyResource>, IGet<MachinePolicyResource>, IDelete<MachinePolicyResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<MachineResource>> GetMachines(MachinePolicyResource machinePolicy);
        Task<List<MachineResource>> GetMachines(MachinePolicyResource machinePolicy, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<MachinePolicyResource> GetTemplate();
        Task<MachinePolicyResource> GetTemplate(CancellationToken cancellationToken);

    }

    class MachinePolicyRepository : BasicRepository<MachinePolicyResource>, IMachinePolicyRepository
    {
        public MachinePolicyRepository(IOctopusAsyncRepository repository) : base(repository, "MachinePolicies")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<MachineResource>> GetMachines(MachinePolicyResource machinePolicy)
            => GetMachines(machinePolicy, CancellationToken.None);

        public async Task<List<MachineResource>> GetMachines(MachinePolicyResource machinePolicy, CancellationToken cancellationToken)
        {
            var resources = new List<MachineResource>();

            await Client.Paginate<MachineResource>(machinePolicy.Link("Machines"), new { }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            }, cancellationToken).ConfigureAwait(false);

            return resources;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MachinePolicyResource> GetTemplate()
            => GetTemplate(CancellationToken.None);

        public async Task<MachinePolicyResource> GetTemplate(CancellationToken cancellationToken)
        {
            var link = await Repository.Link("MachinePolicyTemplate").ConfigureAwait(false);
            return await Client.Get<MachinePolicyResource>(link, cancellationToken).ConfigureAwait(false);
        }
    }
}
