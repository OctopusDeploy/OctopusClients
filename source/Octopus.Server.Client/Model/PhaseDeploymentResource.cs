using System;

namespace Octopus.Client.Model
{
    public class PhaseDeploymentResource
    {
        public TaskResource Task { get; set; }
        public DeploymentResource Deployment { get; set; }
    }
}