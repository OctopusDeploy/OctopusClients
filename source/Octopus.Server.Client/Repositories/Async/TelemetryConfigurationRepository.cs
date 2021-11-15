using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ITelemetryConfigurationRepository
    {
        Task<TelemetryConfigurationResource> GetTelemetryConfiguration();
        Task<TelemetryConfigurationResource> ModifyTelemetryConfiguration(TelemetryConfigurationResource resource);
        Task<TelemetryConfigurationResource> EnableTelemetry();
        Task<TelemetryConfigurationResource> DisableTelemetry();
    }
    
    public class TelemetryConfigurationRepository : ITelemetryConfigurationRepository
    {
        private readonly IOctopusAsyncRepository repository;
        private const string LinkName = "TelemetryConfiguration";

        public TelemetryConfigurationRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
        }

        public async Task<TelemetryConfigurationResource> GetTelemetryConfiguration()
        {
            return await repository.Client.Get<TelemetryConfigurationResource>(await repository.Link(LinkName).ConfigureAwait(false)).ConfigureAwait(false);
        }

        public async Task<TelemetryConfigurationResource> ModifyTelemetryConfiguration(TelemetryConfigurationResource resource)
        {
            return await repository.Client.Update(await repository.Link(LinkName).ConfigureAwait(false), resource).ConfigureAwait(false);
        }

        public async Task<TelemetryConfigurationResource> EnableTelemetry()
        {
            return await repository.Client.Update(await repository.Link(LinkName).ConfigureAwait(false), new TelemetryConfigurationResource
            {
                Enabled = true
            }).ConfigureAwait(false);
        }

        public async Task<TelemetryConfigurationResource> DisableTelemetry()
        {
            return await repository.Client.Update(await repository.Link(LinkName).ConfigureAwait(false), new TelemetryConfigurationResource
            {
                Enabled = false
            }).ConfigureAwait(false);
        }
    }
}