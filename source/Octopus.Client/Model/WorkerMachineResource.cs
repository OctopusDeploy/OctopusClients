using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Model
{
    public class WorkerMachineResource : MachineResource
    {
        public WorkerMachineResource()
        {
            WorkerPoolIds = new ReferenceCollection();
        }

        [Writeable]
        public ReferenceCollection WorkerPoolIds { get; set; }

        public WorkerMachineResource AddOrUpdateWorkerPools(params WorkerPoolResource[] pools)
        {
            foreach (var pool in pools)
            {
                WorkerPoolIds.Add(pool.Id);
            }
            return this;
        }

        public WorkerMachineResource RemoveWorkerPool(WorkerPoolResource pool)
        {
            WorkerPoolIds.Remove(pool.Id);
            return this;
        }

        public WorkerMachineResource ClearWorkerPools()
        {
            WorkerPoolIds.Clear();
            return this;
        }
    }
}