using System;
using System.Collections.Generic;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Serialization
{
    /// <summary>
    /// Serializes <see cref="EndpointResource" />s by including and the CommunicationStyle property.
    /// </summary>
    public class EndpointConverter : InheritedClassConverter<EndpointResource, CommunicationStyle>
    {
        static readonly IDictionary<CommunicationStyle, Type> EndpointTypes =
          new Dictionary<CommunicationStyle, Type>
          {
                {CommunicationStyle.TentacleActive, typeof (PollingTentacleEndpointResource)},
                {CommunicationStyle.TentaclePassive, typeof (ListeningTentacleEndpointResource)},
                {CommunicationStyle.Ssh, typeof (SshEndpointResource)},
                {CommunicationStyle.OfflineDrop, typeof (OfflineDropEndpointResource)},
                {CommunicationStyle.AzureCloudService, typeof (CloudServiceEndpointResource)},
                {CommunicationStyle.AzureWebApp, typeof (AzureWebAppEndpointResource)},
                {CommunicationStyle.None, typeof (CloudRegionEndpointResource)},
                {CommunicationStyle.Kubernetes, typeof (KubernetesEndpointResource)},
                {CommunicationStyle.AzureServiceFabricCluster, typeof(ServiceFabricEndpointResource)},
                {CommunicationStyle.StepPackage, typeof(StepPackageEndpointResource)},
                {CommunicationStyle.KubernetesAgent, typeof(KubernetesEndpointResource)}
          };

        protected override IDictionary<CommunicationStyle, Type> DerivedTypeMappings => EndpointTypes;
        protected override string TypeDesignatingPropertyName => "CommunicationStyle";
    }
}