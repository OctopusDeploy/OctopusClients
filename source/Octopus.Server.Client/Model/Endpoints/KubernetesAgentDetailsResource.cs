namespace Octopus.Client.Model.Endpoints
{
    public class KubernetesAgentDetailsResource
    {
        public string Version { get; set; }

        public string TentacleVersion { get; set; }

        public string HelmReleaseName { get; set; }

        public string Namespace { get; set; }
    }
}