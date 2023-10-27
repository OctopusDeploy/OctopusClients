namespace Octopus.Client.Model.Endpoints
{
    public class PollingTentacleEndpointConfigurationResource : TentacleEndpointConfigurationResource, IPollingTentacleEndpointResource
    {
        public override AgentCommunicationStyleResource CommunicationStyleResource => AgentCommunicationStyleResource.Polling;

        protected PollingTentacleEndpointConfigurationResource()
        {
        }

        public PollingTentacleEndpointConfigurationResource(string thumbprint, string uri) : base(thumbprint, uri)
        {
        }
    }
}