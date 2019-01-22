using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Model;

namespace Octopus.Client
{
    public enum SpaceSelection
    {
        AllSpaces,
        SpecificSpaces
    }

    public class SpaceContext
    {
        public static SpaceContext AllSpaces() => new SpaceContext(SpaceSelection.AllSpaces, new SpaceResource[0], false);
        public static SpaceContext AllSpacesAndSystem() => new SpaceContext(SpaceSelection.AllSpaces, new SpaceResource[0], true);
        public static SpaceContext SpecificSpace(SpaceResource space) => new SpaceContext(SpaceSelection.SpecificSpaces, new [] {space}, false);
        public static SpaceContext SpecificSpaceAndSystem(SpaceResource space) => new SpaceContext(SpaceSelection.SpecificSpaces, new []{space}, true);
        public static SpaceContext SpecificSpaces(IEnumerable<SpaceResource> spaces) => new SpaceContext(SpaceSelection.SpecificSpaces, spaces.ToArray(), false);
        public static SpaceContext SpecificSpacesAndSystem(IEnumerable<SpaceResource> spaces) => new SpaceContext(SpaceSelection.SpecificSpaces, spaces.ToArray(), true);
        public static SpaceContext SpecificSpaces(params SpaceResource[] spaces) => new SpaceContext(SpaceSelection.SpecificSpaces, spaces, false);
        public static SpaceContext SpecificSpacesAndSystem(params SpaceResource[] spaces) => new SpaceContext(SpaceSelection.SpecificSpaces, spaces, true);
        public static SpaceContext SystemOnly() => new SpaceContext(SpaceSelection.SpecificSpaces, new SpaceResource[0], true);
        
        private readonly SpaceSelection spaceSelection;
        private readonly IReadOnlyCollection<SpaceResource> spaces;

        private SpaceContext(SpaceSelection spaceSelection, IReadOnlyCollection<SpaceResource> spaces, bool includeSystem)
        {
            this.spaceSelection = spaceSelection;
            this.spaces = spaces;
            this.IncludeSystem = includeSystem;
        }

        public bool IncludeSystem { get; }
        public T ApplySpaceSelection<T>(Func<IReadOnlyCollection<SpaceResource>, T> handleSpecificSpaces, Func<T> handleAllSpaces)
        {
            switch (spaceSelection)
            {
                case SpaceSelection.AllSpaces:
                    return handleAllSpaces();
                case SpaceSelection.SpecificSpaces:
                    return handleSpecificSpaces(spaces);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ApplySpaceSelection(Action<IReadOnlyCollection<SpaceResource>> handleSpecificSpaces, Action handleAllSpaces)
        {
            ApplySpaceSelection(spaces => { handleSpecificSpaces(spaces); return 1; }, () => { handleAllSpaces(); return 1; });
        }
    }
}