using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class KubernetesEndpointResource : AgentlessEndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.Kubernetes;

        [Trim]
        [Writeable]
        public string AccountId { get; set; }

        [Trim]
        [Writeable]
        public string ClientCertificate { get; set; }

        [Trim]
        [Writeable]
        public string ClusterCertificate { get; set; }

        [Trim]
        [Writeable]
        public string ClusterUrl { get; set; }

        [Trim]
        [Writeable]
        public string Namespace { get; set; }

        [Trim]
        [Writeable]
        public string SkipTlsVerification { get; set; }

        [Trim]
        [Writeable]
        public string ProxyId { get; set; }

        [Trim]
        [Writeable]
        public string DefaultWorkerPoolId { get; set; }
    }
}
