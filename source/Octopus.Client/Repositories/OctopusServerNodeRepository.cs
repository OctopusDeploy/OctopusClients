using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IOctopusServerNodeRepository : IModify<OctopusServerNodeResource>, IDelete<OctopusServerNodeResource>, IGet<OctopusServerNodeResource>, IFindByName<OctopusServerNodeResource>
    {
    }
    
    class OctopusServerNodeRepository : BasicRepository<OctopusServerNodeResource>, IOctopusServerNodeRepository
    {
        public OctopusServerNodeRepository(IOctopusRepository repository)
            : base(repository, "OctopusServerNodes")
        {
        }
    }
}