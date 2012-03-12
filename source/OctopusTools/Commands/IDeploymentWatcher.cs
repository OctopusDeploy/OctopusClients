using System;
using System.Collections.Generic;
using OctopusTools.Client;

namespace OctopusTools.Commands
{
    public interface IDeploymentWatcher
    {
        void WaitForDeploymentsToFinish(IOctopusSession session, IEnumerable<string> linksToDeploymentTasks, TimeSpan timeout, TimeSpan deploymentStatusCheckSleepCycle);
    }
}