using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private readonly SpaceSelection spaceSelection;
        private readonly IReadOnlyCollection<string> spaceIds;

        private SpaceContext(SpaceSelection spaceSelection, IReadOnlyCollection<string> spaceIds, bool includeSystem)
        {
            if (spaceIds.Count == 0 && !includeSystem)
                throw new ArgumentException("At least 1 spaceId is required when includeSystem is set to false");
            this.spaceSelection = spaceSelection;
            this.spaceIds = spaceIds;
            this.IncludeSystem = includeSystem;
        }

        public bool IncludeSystem { get; }
        public T ApplySpaceSelection<T>(Func<IReadOnlyCollection<string>, T> handleSpecificSpaces, Func<T> handleAllSpaces)
        {
            switch (spaceSelection)
            {
                case SpaceSelection.AllSpaces:
                    return handleAllSpaces();
                case SpaceSelection.SpecificSpaces:
                    return handleSpecificSpaces(spaceIds);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ApplySpaceSelection(Action<IReadOnlyCollection<string>> handleSpecificSpaces, Action handleAllSpaces)
        {
            ApplySpaceSelection(spaces => { handleSpecificSpaces(spaces); return 1; }, () => { handleAllSpaces(); return 1; });
        }
    }
}