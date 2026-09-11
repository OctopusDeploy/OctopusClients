using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IConfigurationRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<T> Get<T>() where T : class, IResource, new();
        Task<T> Get<T>(CancellationToken cancellationToken) where T : class, IResource, new();
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<T> Modify<T>(T configurationResource) where T : class, IResource, new();
        Task<T> Modify<T>(T configurationResource, CancellationToken cancellationToken) where T : class, IResource, new();
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<T> Get<T>() where T : class, IResource, new()
        {
            return Get<T>(CancellationToken.None);
        }

        public async Task<T> Get<T>(CancellationToken cancellationToken) where T : class, IResource, new()
        {
            var instance = new T();
            var configurationItem = await GetConfigurationItem(instance).ConfigureAwait(false);

            return await repository.Client.Get<T>(configurationItem.Link("Values"), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<T> Modify<T>(T configurationResource) where T : class, IResource, new()
        {
            return Modify(configurationResource, CancellationToken.None);
        }

        public async Task<T> Modify<T>(T configurationResource, CancellationToken cancellationToken) where T : class, IResource, new()
        {
            var configurationItem = await GetConfigurationItem(configurationResource).ConfigureAwait(false);
            return await repository.Client.Update(configurationItem.Link("Values"), configurationResource, cancellationToken).ConfigureAwait(false);
        }

        private async Task<ConfigurationItemResource> GetConfigurationItem(IResource instance)
        {
            return await repository.Client.Get<ConfigurationItemResource>(await repository.Link(collectionLinkName).ConfigureAwait(false), new { instance.Id }).ConfigureAwait(false);
        }
    }
}
