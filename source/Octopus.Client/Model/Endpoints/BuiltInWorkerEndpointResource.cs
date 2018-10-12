using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class BuiltInWorkerEndpointResource : AgentlessEndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.BuiltIn;
    }
}