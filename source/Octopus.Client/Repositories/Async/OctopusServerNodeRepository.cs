using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IOctopusServerNodeRepository : IModify<OctopusServerNodeResource>, IDelete<OctopusServerNodeResource>, IGet<OctopusServerNodeResource>, IFindByName<OctopusServerNodeResource>
    {
    }

    class OctopusServerNodeRepository : BasicRepository<OctopusServerNodeResource>, IOctopusServerNodeRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public OctopusServerNodeRepository(IOctopusAsyncRepository repository)
            : base(repository, "OctopusServerNodes")
        {
            this.repository = repository;
        }

        public async Task<List<OctopusServerNodeRunningTasksResource>> Running()
        {
            return await repository.Client.Get<List<OctopusServerNodeRunningTasksResource>>(await repository.Link("OctopusServerNodesRunningTasks"));
        }
    }
}
