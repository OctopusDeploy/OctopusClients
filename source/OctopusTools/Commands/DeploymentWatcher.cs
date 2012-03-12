using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Commands
{
    public class DeploymentWatcher : IDeploymentWatcher
    {
        readonly ILog log;

        public DeploymentWatcher(ILog log)
        {
            this.log = log;
        }

        public void WaitForDeploymentsToFinish(IOctopusSession session, IEnumerable<string> linksToDeploymentTasks, TimeSpan timeout, TimeSpan deploymentStatusCheckSleepCycle)
        {
            IDictionary<string, Task> tasks;
            var stopwatch = Stopwatch.StartNew();
            do
            {
                tasks = linksToDeploymentTasks.ToDictionary(link => link, link => session.Get<Task>(link));
                var allTasksFinished = tasks.Values.All(task => task.IsFinished);
                if (allTasksFinished) break;

                EnsureDeploymentIsNotTakingLongerThanExpected(stopwatch.Elapsed, timeout);

                log.Debug(String.Format("Deployment not yet finished. It's taken {0} so far.", stopwatch.Elapsed));
                Thread.Sleep(deploymentStatusCheckSleepCycle);
            } while (true);

            var failedTasks = tasks.Values.Where(task => !task.FinishedSuccessfully);
            if (failedTasks.Any())
            {
                var message = "{0} of the deployment tasks has failed. Please check Octopus web site for more details. Failed tasks: {1}";
                var taskIds = failedTasks.Aggregate("", (accumulator, task) => accumulator + task.Id + ", ");
                throw new CommandException(String.Format(message, failedTasks.Count(), taskIds));
            }

            log.Debug("Deployment has finished succeessfully");       
        }

        void EnsureDeploymentIsNotTakingLongerThanExpected(TimeSpan elapsedTime, TimeSpan timeout)
        {
            if (elapsedTime > timeout)
            {
                throw new CommandException(String.Format("Deployment has taken longer than  {0}", timeout));
            }
        }
    }
}