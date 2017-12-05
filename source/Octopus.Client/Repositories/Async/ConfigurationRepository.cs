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
        private readonly IOctopusAsyncClient client;
        private readonly string collectionLinkName;

        public ConfigurationRepository(IOctopusAsyncClient client)
        {
            this.client = client;
            this.collectionLinkName = "Configuration";
        }

        public async Task<T> Get<T>() where T : class, IResource, new()
        {
            var instance = new T();
            var configurationItem = await GetConfigurationItem(instance).ConfigureAwait(false);

            return await client.Get<T>(configurationItem.Link("Values")).ConfigureAwait(false);
        }

        public async Task<T> Modify<T>(T configurationResource) where T : class, IResource, new()
        {
            var configurationItem = await GetConfigurationItem(configurationResource).ConfigureAwait(false);
            return await client.Update(configurationItem.Link("Values"), configurationResource);
        }

        private async Task<ConfigurationItemResource> GetConfigurationItem(IResource instance)
        {
            return await client.Get<ConfigurationItemResource>(client.RootDocument.Link(collectionLinkName), new { instance.Id });
        }
    }
}
