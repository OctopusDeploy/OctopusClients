using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class ServiceFabricEndpointResource : AgentlessEndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.AzureServiceFabricCluster;

        [Trim]
        [Writeable]
        public string ConnectionEndpoint { get; set; }

        [Trim]
        [Writeable]
        public AzureServiceFabricSecurityMode SecurityMode { get; set; }

        [Trim]
        [Writeable]
        public string ServerCertThumbprint { get; set; }

        [Trim]
        [Writeable]
        public string ClientCertVariable { get; set; }

        [Trim]
        [Writeable]
        public string CertificateStoreLocation { get; set; }

        [Trim]
        [Writeable]
        public string CertificateStoreName { get; set; }

        [Trim]
        [Writeable]
        public AzureServiceFabricCredentialType AadCredentialType { get; set; }

        [Trim]
        [Writeable]
        public string AadClientCredentialSecret { get; set; }

        [Trim]
        [Writeable]
        public string AadUserCredentialUsername { get; set; }

        [Writeable]
        public SensitiveValue AadUserCredentialPassword { get; set; }

        [Trim]
        [Writeable]
        public string DefaultWorkerPoolId { get; set; }
    }
}