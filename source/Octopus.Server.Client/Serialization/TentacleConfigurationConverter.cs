using System;
using System.Collections.Generic;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Serialization;

public class TentacleConfigurationConverter: InheritedClassConverter<TentacleEndpointConfigurationResource, TentacleCommunicationModeResource>
{
    static readonly IDictionary<TentacleCommunicationModeResource, Type> EndpointTypes =
        new Dictionary<TentacleCommunicationModeResource, Type>
        {
            {TentacleCommunicationModeResource.Listening, typeof (ListeningTentacleEndpointConfigurationResource)},
            {TentacleCommunicationModeResource.Polling, typeof(PollingTentacleEndpointConfigurationResource)}
        };

    protected override IDictionary<TentacleCommunicationModeResource, Type> DerivedTypeMappings => EndpointTypes;
    protected override string TypeDesignatingPropertyName => nameof(TentacleEndpointConfigurationResource.CommunicationMode);
}