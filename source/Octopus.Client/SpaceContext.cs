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
        public static SpaceContext SpecificSpaces(bool includeSystem, params string[] spaceIds) => new SpaceContext(spaceIds, includeSystem);
        public static SpaceContext AllSpaces(bool includeSystem) => new SpaceContext(new []{"all"}, includeSystem);
        public static SpaceContext SpecificSpaceAndSystem(string spaceId) => new SpaceContext(new []{spaceId}, true);
        public static SpaceContext SystemOnly() => new SpaceContext(new string[0], true);

        public SpaceContext(IReadOnlyCollection<string> spaceIds, bool includeSystem)
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
            if (!(SpaceIds.Count == 1 && SpaceIds.Single() != "all"))
            {
                throw new MismatchSpaceContextException("You need to be within a single space context in order to execute this task");
            }
        }
    }
}