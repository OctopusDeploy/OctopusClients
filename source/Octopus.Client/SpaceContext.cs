using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Exceptions;
using Octopus.Client.Util;

namespace Octopus.Client
{
    public enum SpaceSelection
    {
        AllSpaces,
        SpecificSpaces
    }

    public class SpaceContext
    {
        public static SpaceContext AllSpaces() => new SpaceContext(SpaceSelection.AllSpaces, new string[] { }, false);
        public static SpaceContext AllSpacesAndSystem() => new SpaceContext(SpaceSelection.AllSpaces, new string[] { }, true);
        public static SpaceContext SpecificSpace(string spaceId) => new SpaceContext(SpaceSelection.SpecificSpaces, new [] {spaceId}, false);
        public static SpaceContext SpecificSpaceAndSystem(string spaceId) => new SpaceContext(SpaceSelection.SpecificSpaces, new []{spaceId}, true);
        public static SpaceContext SpecificSpaces(IEnumerable<string> spaceIds) => new SpaceContext(SpaceSelection.SpecificSpaces, spaceIds.ToArray(), false);
        public static SpaceContext SpecificSpacesAndSystem(IEnumerable<string> spaceIds) => new SpaceContext(SpaceSelection.SpecificSpaces, spaceIds.ToArray(), true);
        public static SpaceContext SpecificSpaces(params string[] spaceIds) => new SpaceContext(SpaceSelection.SpecificSpaces, spaceIds, false);
        public static SpaceContext SpecificSpacesAndSystem(params string[] spaceIds) => new SpaceContext(SpaceSelection.SpecificSpaces, spaceIds, true);
        public static SpaceContext SystemOnly() => new SpaceContext(SpaceSelection.SpecificSpaces, new string[0], true);

        private SpaceContext(SpaceSelection spaceSelection, IReadOnlyCollection<string> spaceIds, bool includeSystem)
        {
            if (spaceIds.Count == 0 && !includeSystem)
                throw new ArgumentException("At least 1 spaceId is required when includeSystem is set to false");
            SpaceSelection = spaceSelection;
            this.SpaceIds = spaceIds;
            this.IncludeSystem = includeSystem;
        }

        public SpaceSelection SpaceSelection { get; }
        public IReadOnlyCollection<string> SpaceIds { get; } 
        public bool IncludeSystem { get; }

        public SpaceContext Union(SpaceContext spaceContext)
        {
            //TODO: Remove me later
            return new SpaceContext(SpaceSelection.SpecificSpaces, this.SpaceIds.Concat(spaceContext.SpaceIds).Distinct().ToArray(), this.IncludeSystem || spaceContext.IncludeSystem);
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