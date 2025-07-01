using Newtonsoft.Json;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class DeploymentSettingsResource : Resource, IHaveSpaceResource
    {
        public string SpaceId { get; set; }

        public string ProjectId { get; set; }

        [Writeable]
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public DeploymentConnectivityPolicy ConnectivityPolicy { get; set; } = new DeploymentConnectivityPolicy { AllowDeploymentsToNoTargets = false };

        [Writeable]
        public GuidedFailureMode DefaultGuidedFailureMode { get; set; }

        [Writeable]
        public VersioningStrategyResource VersioningStrategy { get; set; }

        [Writeable]
        public string ReleaseNotesTemplate { get; set; }

        [Writeable]
        public bool DefaultToSkipIfAlreadyInstalled { get; set; }

        [Writeable]
        public string DeploymentChangesTemplate { get; set; }

        [Writeable]
        public bool ForcePackageDownload { get; set; }
        
        [Writeable]
        public bool FailTargetDiscovery { get; set; }
    }
}