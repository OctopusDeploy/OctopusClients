namespace Octopus.Client.Repositories.Async
{
    public interface ICanExtendSpaceContext<out T>
    {
        T Including(SpaceContext spaceContext);
    }
}