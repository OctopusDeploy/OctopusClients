namespace Octopus.Client.Util
{
    class SpaceQueryParameters
    {
        public SpaceQueryParameters(bool includeSystem, string[] spaceIds)
        {
            IncludeSystem = includeSystem;
            SpaceIds = spaceIds;
        }
        public bool IncludeSystem { get; }
        public string[] SpaceIds { get; }
    }
}