using System;
using System.Collections.Generic;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Serialization;

public class TentacleConfigurationConverter: InheritedClassConverter<KubernetesTentacleEndpointConfigurationResource, TentacleCommunicationModeResource>
{
    static readonly IDictionary<TentacleCommunicationModeResource, Type> EndpointTypes =
        new Dictionary<TentacleCommunicationModeResource, Type>
        {
            {TentacleCommunicationModeResource.Listening, typeof (ListeningKubernetesTentacleEndpointConfigurationResource)},
            {TentacleCommunicationModeResource.Polling, typeof(PollingKubernetesTentacleEndpointConfigurationResource)}
        };

    protected override IDictionary<TentacleCommunicationModeResource, Type> DerivedTypeMappings => EndpointTypes;
    protected override string TypeDesignatingPropertyName => nameof(KubernetesTentacleEndpointConfigurationResource.CommunicationMode);
}