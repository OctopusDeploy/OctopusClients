using System;
using System.Collections.Generic;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Serialization;

public class TentacleConfigurationConverter: InheritedClassConverter<TentacleEndpointConfiguration, AgentCommunicationBehaviour>
{
    static readonly IDictionary<AgentCommunicationBehaviour, Type> EndpointTypes =
        new Dictionary<AgentCommunicationBehaviour, Type>
        {
            {AgentCommunicationBehaviour.Listening, typeof (ListeningTentacleEndpointConfiguration)},
            {AgentCommunicationBehaviour.Polling, typeof(PollingTentacleEndpointConfiguration)}
        };

    protected override IDictionary<AgentCommunicationBehaviour, Type> DerivedTypeMappings => EndpointTypes;
    protected override string TypeDesignatingPropertyName => nameof(TentacleEndpointConfiguration.CommunicationBehaviour);
}