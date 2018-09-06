namespace Octopus.Client.Util
{
    public class SpaceContextExtension
    {
        public SpaceContextExtension(bool includeGlobal, string[] spaceIds)
        {
            IncludeSystem = includeGlobal;
            SpaceIds = spaceIds;
        }
        public bool IncludeSystem { get; }
        public string[] SpaceIds { get; }
    }
}