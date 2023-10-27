namespace Octopus.Client.Model.Endpoints
{
    public class PollingTentacleEndpointConfigurationResource : TentacleEndpointConfigurationResource, IPollingTentacleEndpointResource
    {
        public override AgentCommunicationModeResource CommunicationMode => AgentCommunicationModeResource.Polling;

        protected PollingTentacleEndpointConfigurationResource()
        {
        }

        public PollingTentacleEndpointConfigurationResource(string thumbprint, string uri) : base(thumbprint, uri)
        {
        }
    }
}