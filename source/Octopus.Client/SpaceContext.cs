using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Exceptions;
using Octopus.Client.Util;

namespace Octopus.Client
{
    public class SpaceContext
    {

        public static SpaceContext SpecificSpace(string spaceId) => new SpaceContext(new [] {spaceId}, false);
        public static SpaceContext SpecificSpaceAndSystem(string spaceId) => new SpaceContext(new []{spaceId}, true);
        public static SpaceContext SystemOnly() => new SpaceContext(new string[0], true);

        internal SpaceContext(IReadOnlyCollection<string> spaceIds, bool includeSystem)
        {
            if (spaceIds.Count == 0 && !includeSystem)
                throw new ArgumentException("At least 1 spaceId is required when includeSystem is set to false");
            this.SpaceIds = spaceIds;
            this.IncludeSystem = includeSystem;
        }

        public IReadOnlyCollection<string> SpaceIds { get; } 
        public bool IncludeSystem { get; }

        public SpaceContext Union(SpaceContext spaceContext)
        {
            return new SpaceContext(this.SpaceIds.Concat(spaceContext.SpaceIds).ToArray(), this.IncludeSystem || spaceContext.IncludeSystem);
        }

        public void EnsureSingleSpaceContext()
        {
            if (!(SpaceIds.Count == 1 && SpaceIds.Single() != MixedScopeConstants.AllSpacesQueryStringParameterValue))
            {
                throw new MismatchSpaceContextException("You need to be within a single space context in order to execute this task");
            }
        }
    }
}