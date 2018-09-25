using System.Threading.Tasks;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IConfigurationRepository
    {
        Task<T> Get<T>() where T : class, IResource, new();
        Task<T> Modify<T>(T configurationResource) where T : class, IResource, new();
    }

    class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IOctopusAsyncRepository repository;
        private readonly string collectionLinkName;

        public ConfigurationRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
            this.collectionLinkName = "Configuration";
        }

        public async Task<T> Get<T>() where T : class, IResource, new()
        {
            var instance = new T();
            var configurationItem = await GetConfigurationItem(instance).ConfigureAwait(false);

            return await repository.Client.Get<T>(configurationItem.Link("Values")).ConfigureAwait(false);
        }

        public async Task<T> Modify<T>(T configurationResource) where T : class, IResource, new()
        {
            var configurationItem = await GetConfigurationItem(configurationResource).ConfigureAwait(false);
            return await repository.Client.Update(configurationItem.Link("Values"), configurationResource);
        }

        private async Task<ConfigurationItemResource> GetConfigurationItem(IResource instance)
        {
            return await repository.Client.Get<ConfigurationItemResource>(repository.Link(collectionLinkName), new { instance.Id });
        }
    }
}
