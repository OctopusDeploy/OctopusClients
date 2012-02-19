using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OctopusTools.Client;
using OctopusTools.Commands;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Tests
{
    [TestClass]
    public class deployment_watcher
    {
        [TestMethod]
        public void should_wait_for_deployments_to_finish()
        {
            var log = Substitute.For<ILog>();
            var session = Substitute.For<IOctopusSession>();
            session.Get<Task>(Arg.Any<String>()).Returns(CreateTask("Executing"), CreateTask("Executing"), CreateTask("Success"));

            var watcher = new DeploymentWatcher(session, log);

            var stopwatch = Stopwatch.StartNew();
            watcher.WaitForDeploymentsToFinish(new []{"link"}, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(0.5));
            stopwatch.Stop();

            Assert.IsTrue(stopwatch.Elapsed >= TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        [ExpectedException(typeof(CommandException))]
        public void should_raise_error_when_deployments_take_longer_than_expected()
        {
            var log = Substitute.For<ILog>();
            var session = Substitute.For<IOctopusSession>();
            session.Get<Task>(Arg.Any<String>()).Returns(new Task { State = "Executing" });

            var watcher = new DeploymentWatcher(session, log);

            watcher.WaitForDeploymentsToFinish(new[] { "link" }, TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5));
        }

        [TestMethod]
        [ExpectedException(typeof(CommandException))]
        public void should_raise_error_when_one_of_the_deployment_tasks_failed()
        {
            var log = Substitute.For<ILog>();
            var session = Substitute.For<IOctopusSession>();
            session.Get<Task>(Arg.Any<String>()).Returns(new Task { State = "Failed" });

            var watcher = new DeploymentWatcher(session, log);

            watcher.WaitForDeploymentsToFinish(new[] { "link" }, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(0.5));
        }

        Task CreateTask(string state)
        {
            return new Task() { State = state };
        }
    }
}
