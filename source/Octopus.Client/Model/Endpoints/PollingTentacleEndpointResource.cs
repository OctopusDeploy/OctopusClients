using System;

namespace Octopus.Client.Model.Endpoints
{
    public class PollingTentacleEndpointResource : TentacleEndpointResource
    {
        public override CommunicationStyle CommunicationStyle
        {
            get { return CommunicationStyle.TentacleActive; }
        }

        [Trim]
        [Writeable]
        public string Uri { get; set; }
    }
}