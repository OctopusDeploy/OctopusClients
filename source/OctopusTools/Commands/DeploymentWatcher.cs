using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using OctopusTools.Client;
using OctopusTools.Extensions;
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

        public void WaitForDeploymentsToFinish(IOctopusSession session, IList<string> linksToDeploymentTasks, TimeSpan timeout, TimeSpan deploymentStatusCheckSleepCycle)
        {
            var tasks = WaitUntilTasksFinish(session, linksToDeploymentTasks, timeout, deploymentStatusCheckSleepCycle);

            var failedTasks = tasks.Where(task => !task.FinishedSuccessfully).ToList();
            if (failedTasks.Any())
            {
                var message = new StringBuilder();
                foreach (var task in failedTasks)
                {
                    message.AppendFormat("The task: '{0}' failed with the error: {1}", task.Description, task.ErrorMessage).AppendLine();
                    message.AppendFormat("Please see the deployment page for more details: {0}", session.QualifyWebLink(task.Link("Web"))).AppendLine();
                }

                throw new CommandException(message.ToString());
            }

            log.Debug("Deployment finished successfully!");       
        }

        IEnumerable<Task> WaitUntilTasksFinish(IOctopusSession session, IList<string> linksToDeploymentTasks, TimeSpan timeout, TimeSpan deploymentStatusCheckSleepCycle)
        {
            var stopwatch = Stopwatch.StartNew();

            while (true)
            {
                var tasks = FetchTasks(session, linksToDeploymentTasks);

                var allFinished = tasks.All(task => task.IsFinished);
                if (allFinished)
                    return tasks;

                EnsureDeploymentIsNotTakingLongerThanExpected(stopwatch.Elapsed, timeout);

                log.DebugFormat("Deployment has not yet finished. Time taken: {0}", stopwatch.Elapsed.Friendly());

                Thread.Sleep(deploymentStatusCheckSleepCycle);
            }
        }

        static IList<Task> FetchTasks(IOctopusSession session, IEnumerable<string> linksToDeploymentTasks)
        {
            return linksToDeploymentTasks.Select(session.Get<Task>).ToList();
        }

        static void EnsureDeploymentIsNotTakingLongerThanExpected(TimeSpan elapsedTime, TimeSpan timeout)
        {
            if (elapsedTime > timeout)
            {
                throw new CommandException(String.Format("Deployment has taken longer than  {0}", timeout));
            }
        }
    }
}