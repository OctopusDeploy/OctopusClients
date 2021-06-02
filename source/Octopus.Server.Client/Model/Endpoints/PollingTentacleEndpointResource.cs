using System;
using Octopus.Client.Extensibility.Attributes;

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