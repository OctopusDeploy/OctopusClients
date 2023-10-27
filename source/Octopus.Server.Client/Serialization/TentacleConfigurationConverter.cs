using System;
using System.Collections.Generic;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Serialization;

public class TentacleConfigurationConverter: InheritedClassConverter<TentacleEndpointConfigurationResource, AgentCommunicationStyleResource>
{
    static readonly IDictionary<AgentCommunicationStyleResource, Type> EndpointTypes =
        new Dictionary<AgentCommunicationStyleResource, Type>
        {
            {AgentCommunicationStyleResource.Listening, typeof (ListeningTentacleEndpointConfigurationResource)},
            {AgentCommunicationStyleResource.Polling, typeof(PollingTentacleEndpointConfigurationResource)}
        };

    protected override IDictionary<AgentCommunicationStyleResource, Type> DerivedTypeMappings => EndpointTypes;
    protected override string TypeDesignatingPropertyName => nameof(TentacleEndpointConfigurationResource.CommunicationStyleResource);
}