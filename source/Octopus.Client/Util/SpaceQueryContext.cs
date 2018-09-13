namespace Octopus.Client.Util
{
    class SpaceQueryContext
    {
        public SpaceQueryContext(bool includeSystem, string[] spaceIds)
        {
            IncludeSystem = includeSystem;
            SpaceIds = spaceIds;
        }
        public bool IncludeSystem { get; }
        public string[] SpaceIds { get; }
    }
}