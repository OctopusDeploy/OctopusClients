using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Util;

namespace Octopus.Client
{
    public class SpaceContext
    {
        public static SpaceContext SpecificSpace(string spaceId) => new SpaceContext(new [] {spaceId}, false);
        public static SpaceContext SpecificSpaceAndSystem(string spaceId) => new SpaceContext(new []{spaceId}, true);
        public static SpaceContext SystemOnly() => new SpaceContext(new string[0], true);

        public SpaceContext(IReadOnlyCollection<string> spaceIds, bool includeSystemPartition)
        {
            this.SpaceIds = spaceIds;
            this.IncludeSystem = includeSystemPartition;
        }

        public IReadOnlyCollection<string> SpaceIds { get; } 
        public bool IncludeSystem { get; }

        public SpaceContext Union(SpaceContext spaceContext)
        {
            return new SpaceContext(this.SpaceIds.Concat(spaceContext.SpaceIds).ToArray(), this.IncludeSystem || spaceContext.IncludeSystem);
        }

        public SpaceContextExtension ToSpaceContextExtension()
        {
            return new SpaceContextExtension(IncludeSystem, SpaceIds.ToArray());
        }
    }
}