using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Model
{
    public class WorkerResource : MachineBasedResource
    {
        public WorkerResource()
        {
            WorkerPoolIds = new ReferenceCollection();
        }

        [Writeable]
        public ReferenceCollection WorkerPoolIds { get; set; }

        public WorkerResource AddOrUpdateWorkerPools(params WorkerPoolResource[] pools)
        {
            foreach (var pool in pools)
            {
                WorkerPoolIds.Add(pool.Id);
            }
            return this;
        }

        public WorkerResource RemoveWorkerPool(WorkerPoolResource pool)
        {
            WorkerPoolIds.Remove(pool.Id);
            return this;
        }

        public WorkerResource ClearWorkerPools()
        {
            WorkerPoolIds.Clear();
            return this;
        }
    }
}