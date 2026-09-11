using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ITelemetryConfigurationRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TelemetryConfigurationResource> GetTelemetryConfiguration();
        Task<TelemetryConfigurationResource> GetTelemetryConfiguration(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TelemetryConfigurationResource> ModifyTelemetryConfiguration(TelemetryConfigurationResource resource);
        Task<TelemetryConfigurationResource> ModifyTelemetryConfiguration(TelemetryConfigurationResource resource, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TelemetryConfigurationResource> EnableTelemetry();
        Task<TelemetryConfigurationResource> EnableTelemetry(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TelemetryConfigurationResource> DisableTelemetry();
        Task<TelemetryConfigurationResource> DisableTelemetry(CancellationToken cancellationToken);
    }

    public class TelemetryConfigurationRepository : ITelemetryConfigurationRepository
    {
        private readonly IOctopusAsyncRepository repository;
        private const string LinkName = "TelemetryConfiguration";

        public TelemetryConfigurationRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TelemetryConfigurationResource> GetTelemetryConfiguration()
            => GetTelemetryConfiguration(CancellationToken.None);

        public async Task<TelemetryConfigurationResource> GetTelemetryConfiguration(CancellationToken cancellationToken)
        {
            return await repository.Client.Get<TelemetryConfigurationResource>(await repository.Link(LinkName).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TelemetryConfigurationResource> ModifyTelemetryConfiguration(TelemetryConfigurationResource resource)
            => ModifyTelemetryConfiguration(resource, CancellationToken.None);

        public async Task<TelemetryConfigurationResource> ModifyTelemetryConfiguration(TelemetryConfigurationResource resource, CancellationToken cancellationToken)
        {
            return await repository.Client.Update(await repository.Link(LinkName).ConfigureAwait(false), resource, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TelemetryConfigurationResource> EnableTelemetry()
            => EnableTelemetry(CancellationToken.None);

        public async Task<TelemetryConfigurationResource> EnableTelemetry(CancellationToken cancellationToken)
        {
            return await ModifyTelemetryConfiguration(new TelemetryConfigurationResource
            {
                Enabled = true
            }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TelemetryConfigurationResource> DisableTelemetry()
            => DisableTelemetry(CancellationToken.None);

        public async Task<TelemetryConfigurationResource> DisableTelemetry(CancellationToken cancellationToken)
        {
            return await ModifyTelemetryConfiguration(new TelemetryConfigurationResource
            {
                Enabled = false
            }, cancellationToken);
        }
    }
}
