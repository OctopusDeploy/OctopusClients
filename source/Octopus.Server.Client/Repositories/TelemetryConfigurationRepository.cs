using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITelemetryConfigurationRepository
    {
        TelemetryConfigurationResource GetTelemetryConfiguration();
        TelemetryConfigurationResource ModifyTelemetryConfiguration(TelemetryConfigurationResource resource);
        TelemetryConfigurationResource EnableTelemetry();
        TelemetryConfigurationResource DisableTelemetry();
    }
    
    public class TelemetryConfigurationRepository : ITelemetryConfigurationRepository
    {
        private readonly IOctopusRepository repository;
        private const string LinkName = "TelemetryConfiguration";

        public TelemetryConfigurationRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public TelemetryConfigurationResource GetTelemetryConfiguration()
        {
            return repository.Client.Get<TelemetryConfigurationResource>(repository.Link(LinkName));
        }

        public TelemetryConfigurationResource ModifyTelemetryConfiguration(TelemetryConfigurationResource resource)
        {
            return repository.Client.Update(repository.Link(LinkName), resource);
        }

        public TelemetryConfigurationResource EnableTelemetry()
        {
            return ModifyTelemetryConfiguration(new TelemetryConfigurationResource
            {
                Enabled = true
            });
        }

        public TelemetryConfigurationResource DisableTelemetry()
        {
            return ModifyTelemetryConfiguration(new TelemetryConfigurationResource
            {
                Enabled = false
            });
        }
    }
}