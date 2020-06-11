using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class ListeningTentacleEndpointResource : TentacleEndpointResource
    {
        public override CommunicationStyle CommunicationStyle
        {
            get { return CommunicationStyle.TentaclePassive; }
        }

        [Writeable]
        public string ProxyId { get; set; }
    }
}