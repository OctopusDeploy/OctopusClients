namespace Octopus.Client.Model.Endpoints
{
    public class PollingKubernetesTentacleEndpointConfigurationResource : KubernetesTentacleEndpointConfigurationResource, IPollingTentacleEndpointResource
    {
        public override TentacleCommunicationModeResource CommunicationMode => TentacleCommunicationModeResource.Polling;

        protected PollingKubernetesTentacleEndpointConfigurationResource()
        {
        }

        public PollingKubernetesTentacleEndpointConfigurationResource(string thumbprint, string uri) : base(thumbprint, uri)
        {
        }
    }
}