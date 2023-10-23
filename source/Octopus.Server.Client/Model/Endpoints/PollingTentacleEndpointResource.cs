using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class PollingTentacleEndpointResource : TentacleEndpointResource, IPollingTentacleEndpointConfiguration
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.TentacleActive;
    }

    public interface IPollingTentacleEndpointConfiguration : ITentacleEndpointConfiguration
    {
    }
}