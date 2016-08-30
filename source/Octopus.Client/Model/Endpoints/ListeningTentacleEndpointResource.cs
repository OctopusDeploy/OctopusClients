using System;

namespace Octopus.Client.Model.Endpoints
{
    public class ListeningTentacleEndpointResource : TentacleEndpointResource
    {
        public override CommunicationStyle CommunicationStyle
        {
            get { return CommunicationStyle.TentaclePassive; }
        }

        [Trim]
        [Writeable]
        public string Uri { get; set; }

        [Writeable]
        public string ProxyId { get; set; }
    }
}