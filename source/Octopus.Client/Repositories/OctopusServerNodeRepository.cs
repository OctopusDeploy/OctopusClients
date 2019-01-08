using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IOctopusServerNodeRepository : IModify<OctopusServerNodeResource>, IDelete<OctopusServerNodeResource>, IGet<OctopusServerNodeResource>, IFindByName<OctopusServerNodeResource>
    {
        OctopusServerNodeDetailsResource Details(OctopusServerNodeResource node);
    }
    
    class OctopusServerNodeRepository : BasicRepository<OctopusServerNodeResource>, IOctopusServerNodeRepository
    {
        private readonly IOctopusRepository repository;

        public OctopusServerNodeRepository(IOctopusRepository repository)
            : base(repository, "OctopusServerNodes")
        {
            this.repository = repository;
        }

        public OctopusServerNodeDetailsResource Details(OctopusServerNodeResource node)
        {
            return repository.Client.Get<OctopusServerNodeDetailsResource>(node.Link("Details"));
        }
    }
}