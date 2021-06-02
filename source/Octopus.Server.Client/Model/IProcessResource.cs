using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public interface IProcessResource : IResource
    {
        string ProjectId { get; set; }
        IList<DeploymentStepResource> Steps { get; }
        int Version { get; set; }
        string LastSnapshotId { get; set; }

        DeploymentStepResource FindStep(string name);
        DeploymentStepResource AddOrUpdateStep(string name);
        IProcessResource RemoveStep(string name);
        IProcessResource ClearSteps();
    }
}
