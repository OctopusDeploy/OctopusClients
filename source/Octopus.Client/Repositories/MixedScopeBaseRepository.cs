using System;
using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    abstract class MixedScopeBaseRepository<TMixedScopeResource> : BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        protected MixedScopeBaseRepository(IOctopusClient client, string collectionLinkName) : base(client, collectionLinkName)
        {
        }

        protected SpaceContextExtension SpaceContextExtension { get; set; }

        protected SpaceContext CreateSpaceContext(SpaceContext spaceContext)
        {
            if (Client.SpaceContext.IncludeSystem && !spaceContext.IncludeSystem)
            {
                throw new ArgumentException("Cannot reset includeGlobal to false while it is already set to true");
            }
            return new SpaceContext(Client.SpaceContext.SpaceIds.Concat(spaceContext.SpaceIds).ToArray(), spaceContext.IncludeSystem);
        }
    }
}
