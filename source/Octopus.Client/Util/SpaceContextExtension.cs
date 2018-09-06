namespace Octopus.Client.Util
{
    public class SpaceContextExtension
    {
        public SpaceContextExtension(bool includeSystem, string[] spaceIds)
        {
            IncludeSystem = includeSystem;
            SpaceIds = spaceIds;
        }
        public bool IncludeSystem { get; }
        public string[] SpaceIds { get; }
    }
}