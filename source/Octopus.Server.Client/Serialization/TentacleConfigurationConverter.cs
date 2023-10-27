using System;
using System.Collections.Generic;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Serialization;

public class TentacleConfigurationConverter: InheritedClassConverter<TentacleEndpointConfigurationResource, AgentCommunicationModeResource>
{
    static readonly IDictionary<AgentCommunicationModeResource, Type> EndpointTypes =
        new Dictionary<AgentCommunicationModeResource, Type>
        {
            {AgentCommunicationModeResource.Listening, typeof (ListeningTentacleEndpointConfigurationResource)},
            {AgentCommunicationModeResource.Polling, typeof(PollingTentacleEndpointConfigurationResource)}
        };

    protected override IDictionary<AgentCommunicationModeResource, Type> DerivedTypeMappings => EndpointTypes;
    protected override string TypeDesignatingPropertyName => nameof(TentacleEndpointConfigurationResource.CommunicationMode);
}