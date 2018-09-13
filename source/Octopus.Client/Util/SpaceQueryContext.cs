namespace Octopus.Client.Util
{
    class SpaceQueryContext
    {
        public SpaceQueryContext(bool includeGlobal, string[] spaceIds)
        {
            IncludeGlobal = includeGlobal;
            SpaceIds = spaceIds;
        }
        public bool IncludeGlobal { get; }
        public string[] SpaceIds { get; }
    }
}