﻿using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class KubernetesEndpointResource : EndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.Kubernetes;

        [Trim]
        [Writeable]
        public string ClusterCertificate { get; set; }
        
        [Trim]
        [Writeable]
        public string ClusterCertificatePath { get; set; }

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
        public string ContainerOptions { get; set; }

        [Trim]
        [Writeable]
        public string DefaultWorkerPoolId { get; set; }

        [Writeable]
        public IEndpointWithMultipleAuthenticationResource Authentication { get; set; }

        [Writeable]
        public DeploymentActionContainerResource Container { get; set; }
    }
}
