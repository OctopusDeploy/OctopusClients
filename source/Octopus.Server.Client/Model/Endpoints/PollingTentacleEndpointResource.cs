using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class PollingTentacleEndpointResource : TentacleEndpointResource, IPollingTentacleEndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.TentacleActive;
    }

    public interface IPollingTentacleEndpointResource : ITentacleEndpointResource
    {
    }
}