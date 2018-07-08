using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IConfigurationRepository
    {
        T Get<T>() where T : class, IResource, new();
        T Modify<T>(T configurationResource) where T : class, IResource, new();
    }

    class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IOctopusClient client;
        private readonly string collectionLinkName;

        public ConfigurationRepository(IOctopusClient client)
        {
            this.client = client;
            this.collectionLinkName = "Configuration";
        }

        public T Get<T>() where T : class, IResource, new()
        {
            var instance = new T();
            var configurationItem = GetConfigurationItem(instance);
            return client.Get<T>(configurationItem.Link("Values"));
        }

        public T Modify<T>(T configurationResource) where T : class, IResource, new()
        {
            var configurationItem = GetConfigurationItem(configurationResource);
            return client.Update(configurationItem.Link("Values"), configurationResource);
        }

        private ConfigurationItemResource GetConfigurationItem(IResource instance) 
        {
            var configurationItem =
                client.Get<ConfigurationItemResource>(client.Link(collectionLinkName), new { instance.Id });
            return configurationItem;
        }
    }
}