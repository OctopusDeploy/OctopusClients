using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IOctopusServerNodeRepository : IModify<OctopusServerNodeResource>, IDelete<OctopusServerNodeResource>, IGet<OctopusServerNodeResource>, IFindByName<OctopusServerNodeResource>
    {
    }
    
    class OctopusServerNodeRepository : BasicRepository<OctopusServerNodeResource>, IOctopusServerNodeRepository
    {
        private readonly IOctopusRepository repository;

        public OctopusServerNodeRepository(IOctopusRepository repository)
            : base(repository, "OctopusServerNodes")
        {
            this.repository = repository;
        }

        public List<OctopusServerNodeDetailsResource> Running()
        {
            return repository.Client.Get<List<OctopusServerNodeDetailsResource>>(repository.Link("OctopusServerNodesRunningTasks"));
        }
    }
}