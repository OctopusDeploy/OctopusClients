using System;
using System.Collections.Generic;

namespace OctopusTools.Commands
{
    public interface IDeploymentWatcher
    {
        void WaitForDeploymentsToFinish(IEnumerable<string> linksToDeploymentTasks, TimeSpan timeout, TimeSpan deploymentStatusCheckSleepCycle);
    }
}