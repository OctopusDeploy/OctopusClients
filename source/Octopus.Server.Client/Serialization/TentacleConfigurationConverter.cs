using System;
using System.Collections.Generic;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Serialization;

public class TentacleConfigurationConverter: InheritedClassConverter<TentacleEndpointConfiguration, AgentCommunicationStyleResource>
{
    static readonly IDictionary<AgentCommunicationStyleResource, Type> EndpointTypes =
        new Dictionary<AgentCommunicationStyleResource, Type>
        {
            {AgentCommunicationStyleResource.Listening, typeof (ListeningTentacleEndpointConfiguration)},
            {AgentCommunicationStyleResource.Polling, typeof(PollingTentacleEndpointConfiguration)}
        };

    protected override IDictionary<AgentCommunicationStyleResource, Type> DerivedTypeMappings => EndpointTypes;
    protected override string TypeDesignatingPropertyName => nameof(TentacleEndpointConfiguration.CommunicationStyleResource);
}