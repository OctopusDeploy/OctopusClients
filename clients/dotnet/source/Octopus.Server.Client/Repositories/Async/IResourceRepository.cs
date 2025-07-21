namespace Octopus.Client.Repositories.Async
{
    public interface IResourceRepository
    {
        IOctopusAsyncClient Client { get; }
    }
}