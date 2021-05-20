using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class StepPackageEndpointResource : EndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.StepPackage;

        [Trim]
        [Writeable]
        public string Inputs { get; set; }
    }
}