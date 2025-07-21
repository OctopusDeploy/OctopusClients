using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class StepPackageEndpointResource : EndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.StepPackage;

        [Trim]
        [Writeable]
        public string DeploymentTargetTypeId { get; set; }

        [Trim]
        [Writeable]
        public string StepPackageVersion { get; set; }

        [Trim]
        [Writeable]
        public object Inputs { get; set; }

        [Trim]
        [Writeable]
        public string DefaultWorkerPoolId { get; set; }
    }
}