using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class ListeningKubernetesTentacleEndpointConfigurationResource : KubernetesTentacleEndpointConfigurationResource, IListeningTentacleEndpointResource
    {
        protected ListeningKubernetesTentacleEndpointConfigurationResource()
        {
        }

        public ListeningKubernetesTentacleEndpointConfigurationResource(string thumbprint, string uri) : base(thumbprint, uri)
        {
        }

        public override TentacleCommunicationModeResource CommunicationMode => TentacleCommunicationModeResource.Listening;

        [Writeable]
        public string ProxyId { get; set; }
    }
}