namespace Octopus.Client.Repositories.Async
{
    public interface ICanExtendSpaceContext<out T>
    {
        T Including(SpaceContext spaceContext);
    }

    public class SpaceContext
    {
        public IReadOnlyCollection<string> SpaceIds { get; }
        public bool IncludeSystem { get; }
    }
}