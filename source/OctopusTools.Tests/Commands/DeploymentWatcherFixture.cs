using System;
using System.Collections.Generic;
using System.Diagnostics;
using NSubstitute;
using NUnit.Framework;
using OctopusTools.Client;
using OctopusTools.Commands;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class DeploymentWatcherFixture
    {
        [Test]
        public void ShouldWaitForDeploymentsToFinish()
        {
            var log = Substitute.For<ILog>();
            var session = Substitute.For<IOctopusSession>();
            session.Get<Task>(Arg.Any<String>()).Returns(CreateTask("Executing"), CreateTask("Executing"), CreateTask("Success"));

            var watcher = new DeploymentWatcher(log);

            var stopwatch = Stopwatch.StartNew();
            watcher.WaitForDeploymentsToFinish(session, new[] { "link" }, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(0.5));
            stopwatch.Stop();

            Assert.IsTrue(stopwatch.Elapsed >= TimeSpan.FromSeconds(1));
        }

        [Test]
        [ExpectedException(typeof(CommandException))]
        public void ShouldRaiseErrorWhenDeploymentsTakeLongerThanExpected()
        {
            var log = Substitute.For<ILog>();
            var session = Substitute.For<IOctopusSession>();
            session.Get<Task>(Arg.Any<String>()).Returns(new Task { State = "Executing" });

            var watcher = new DeploymentWatcher(log);

            watcher.WaitForDeploymentsToFinish(session, new[] { "link" }, TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5));
        }

        [Test]
        [ExpectedException(typeof(CommandException))]
        public void ShouldRaiseErrorWhenOneOfTheDeploymentTasksFailed()
        {
            var log = Substitute.For<ILog>();
            var session = Substitute.For<IOctopusSession>();
            session.Get<Task>(Arg.Any<String>()).Returns(new Task { State = "Failed", Links = new Dictionary<string, string> { { "Web", "Foo" } } });

            var watcher = new DeploymentWatcher(log);

            watcher.WaitForDeploymentsToFinish(session, new[] { "link" }, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(0.5));
        }

        Task CreateTask(string state)
        {
            return new Task() { State = state };
        }
    }
}
