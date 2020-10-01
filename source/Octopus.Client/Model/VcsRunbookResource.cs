using Newtonsoft.Json;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class VcsRunbookResource : Resource, INamedResource
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public TenantedDeploymentMode MultiTenancyMode { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public ProjectConnectivityPolicy ConnectivityPolicy { get; set; } = new ProjectConnectivityPolicy() { AllowDeploymentsToNoTargets = true};

        public RunbookEnvironmentScope EnvironmentScope { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public ReferenceCollection Environments { get; } = new ReferenceCollection();

        public GuidedFailureMode DefaultGuidedFailureMode { get; set; }
    }
}