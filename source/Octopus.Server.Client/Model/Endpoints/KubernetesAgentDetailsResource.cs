namespace Octopus.Client.Model.Endpoints
{
    public class KubernetesAgentDetailsResource
    {
        public string KubernetesAgentVersion { get; set; }

        public string HelmReleaseName { get; set; }

        public string KubernetesAgentNamespace { get; set; }
    }
}