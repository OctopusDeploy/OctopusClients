namespace Octopus.Client
{
    public class SpaceContext
    {
        public static SpaceContext SpecificSpace(string spaceId) => new SpaceContext(SpaceSelection.SpecificSpace, spaceId, false);
        public static SpaceContext SpecificSpaceAndSystem(string spaceId) => new SpaceContext(SpaceSelection.SpecificSpaceAndSystem, spaceId, true);
        public static SpaceContext DefaultSpaceAndSystem() => new SpaceContext(SpaceSelection.DefaultSpaceAndSystem, null, true);
        public static SpaceContext SystemOnly() => new SpaceContext(SpaceSelection.SystemOnly, null, true);

        public SpaceContext(SpaceSelection spaceSelection, string spaceId, bool includeSystemPartition)
        {
            SpaceSelection = spaceSelection;
            this.SpaceId = spaceId;
            this.IncludeSystem = includeSystemPartition;
        }

        public SpaceSelection SpaceSelection { get; }
        public string SpaceId { get; } 
        public bool IncludeSystem { get; }
    }
}