using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class ListeningTentacleEndpointConfigurationResource : TentacleEndpointConfigurationResource, IListeningTentacleEndpointResource
    {
        protected ListeningTentacleEndpointConfigurationResource()
        {
        }

        public ListeningTentacleEndpointConfigurationResource(string uri, string thumbprint) : base(uri, thumbprint)
        {
        }

        public override AgentCommunicationStyleResource CommunicationStyleResource => AgentCommunicationStyleResource.Listening;

        [Writeable]
        public string ProxyId { get; set; }
    }
}