namespace Octopus.Client.Model.Endpoints
{
    public class KubernetesAgentDetailsResource
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public KubernetesAgentDetailsResource()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public KubernetesAgentDetailsResource(string agentVersion,
            string tentacleVersion,
            UpgradeStatus upgradeStatus,
            string helmReleaseName,
            string kubernetesNamespace)
        {
            AgentVersion = agentVersion;
            TentacleVersion = tentacleVersion;
            UpgradeStatus = upgradeStatus;
            HelmReleaseName = helmReleaseName;
            KubernetesNamespace = kubernetesNamespace;
        }

        public string AgentVersion { get; set; }

        public string TentacleVersion { get; set; }

        public UpgradeStatus UpgradeStatus { get; set; }

        public string HelmReleaseName { get; set; }

        public string KubernetesNamespace { get; set; }
    }
}