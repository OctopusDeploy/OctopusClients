namespace Octopus.Client.Model.Endpoints
{
    public class KubernetesAgentDetailsResource
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public KubernetesAgentDetailsResource()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public KubernetesAgentDetailsResource(string version,
            string tentacleVersion,
            UpgradeStatus upgradeStatus,
            string helmReleaseName,
            string @namespace)
        {
            Version = version;
            TentacleVersion = tentacleVersion;
            UpgradeStatus = upgradeStatus;
            HelmReleaseName = helmReleaseName;
            Namespace = @namespace;
        }

        public string Version { get; set; }

        public string TentacleVersion { get; set; }

        public UpgradeStatus UpgradeStatus { get; set; }

        public string HelmReleaseName { get; set; }

        public string Namespace { get; set; }
    }
}