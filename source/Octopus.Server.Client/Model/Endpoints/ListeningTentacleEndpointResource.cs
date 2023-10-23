using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class ListeningTentacleEndpointResource : TentacleEndpointResource, IListeningTentacleEndpointConfiguration
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.TentaclePassive;

        [Writeable]
        public string ProxyId { get; set; }
    }

    public interface IListeningTentacleEndpointConfiguration : ITentacleEndpointConfiguration
    {
        string ProxyId { get; set; }
    }
}