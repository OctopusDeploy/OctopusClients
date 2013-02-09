using System;
using System.Collections.Generic;
using OctopusTools.Client;

namespace OctopusTools.Commands
{
    public interface IDeploymentWatcher
    {
        void WaitForDeploymentsToFinish(IOctopusSession session, IList<string> linksToDeploymentTasks, TimeSpan timeout, TimeSpan deploymentStatusCheckSleepCycle);
    }
}