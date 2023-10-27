using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class ListeningTentacleEndpointResource : TentacleEndpointResource, IListeningTentacleEndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.TentaclePassive;

        [Writeable]
        public string ProxyId { get; set; }
    }
}