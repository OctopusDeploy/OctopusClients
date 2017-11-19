using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
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

        public void Modify<T>(T configurationResource) where T : class, IResource, new()
        {
            var configResourceInstance = new T();
            var configurationItem = GetConfigurationItem(configurationResource);
            client.Put<T>(configurationItem.Link("Values"), configurationResource);
        }

        private ConfigurationItemResource GetConfigurationItem(IResource instance) 
        {
            var configurationItem =
                client.Get<ConfigurationItemResource>(client.RootDocument.Link(collectionLinkName), new { instance.Id });
            return configurationItem;
        }
    }
}