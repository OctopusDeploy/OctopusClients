namespace Octopus.Client.Model.Endpoints
{
    public class CloudRegionEndpointResource : AgentlessEndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.None;
    }
}