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
        private readonly IOctopusRepository repository;
        private readonly string collectionLinkName;

        public ConfigurationRepository(IOctopusRepository repository)
        {
            this.repository = repository;
            this.collectionLinkName = "Configuration";
        }

        public T Get<T>() where T : class, IResource, new()
        {
            var instance = new T();
            var configurationItem = GetConfigurationItem(instance);
            return repository.Client.Get<T>(configurationItem.Link("Values"));
        }

        public T Modify<T>(T configurationResource) where T : class, IResource, new()
        {
            var configurationItem = GetConfigurationItem(configurationResource);
            return repository.Client.Update(configurationItem.Link("Values"), configurationResource);
        }

        private ConfigurationItemResource GetConfigurationItem(IResource instance) 
        {
            var configurationItem =
                repository.Client.Get<ConfigurationItemResource>(repository.Link(collectionLinkName), new { instance.Id });
            return configurationItem;
        }
    }
}