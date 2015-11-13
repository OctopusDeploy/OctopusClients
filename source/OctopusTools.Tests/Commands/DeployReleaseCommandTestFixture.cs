using NUnit.Framework;
using Octopus.Client.Model;
using OctopusTools.Commands;
using System;
using System.Collections.Generic;
using NSubstitute;
using OctopusTools.Infrastructure;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class DeployReleaseCommandTestFixture: ApiCommandFixtureBase
    {
        DeployReleaseCommand deployReleaseCommand;
        const string ProjectName = "TestProject";
        TaskResource taskResource;

        [SetUp]
        public void SetUp()
        {
            deployReleaseCommand = new DeployReleaseCommand(RepositoryFactory, Log);

            var project = new ProjectResource();
            var release = new ReleaseResource { Version = "1.0.0" };
            var releases = new ResourceCollection<ReleaseResource>(new[] { release }, new LinkCollection());
            var deploymentPromotionTarget = new DeploymentPromotionTarget { Name = "TestEnvironment" };
            var promotionTargets = new List<DeploymentPromotionTarget> { deploymentPromotionTarget };
            var deploymentTemplate = new DeploymentTemplateResource { PromoteTo = promotionTargets };
            var deploymentPreviewResource = new DeploymentPreviewResource { StepsToExecute = new List<DeploymentTemplateStep>() };
            var deployment = new DeploymentResource { TaskId = "1" };
            taskResource = new TaskResource();

            Repository.Projects.FindByName(ProjectName).Returns(project);
            Repository.Projects.GetReleases(project).Returns(releases);
            Repository.Releases.GetPreview(deploymentPromotionTarget).Returns(deploymentPreviewResource);
            Repository.Releases.GetTemplate(release).Returns(deploymentTemplate);
            Repository.Deployments.Create(Arg.Any<DeploymentResource>()).Returns(deployment);
            Repository.Tasks.Get(deployment.TaskId).Returns(taskResource);
        }

        [Test]
        public void ShouldCancelDeploymentOnTimeoutIfRequested()
        {
            Repository.Tasks
                .When(x => x.WaitForCompletion(Arg.Any<TaskResource[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Action<TaskResource[]>>()))
                .Do(x => { throw new TimeoutException(); });

            CommandLineArgs.Add("--project=" + ProjectName);
            CommandLineArgs.Add("--deploymenttimeout=00:00:01");
            CommandLineArgs.Add("--deployto=TestEnvironment");
            CommandLineArgs.Add("--version=latest");
            CommandLineArgs.Add("--progress");
            CommandLineArgs.Add("--cancelontimeout");

            Assert.Throws<CommandException>(() => deployReleaseCommand.Execute(CommandLineArgs.ToArray()));

            Repository.Tasks.Received().Cancel(taskResource);
        }

        [Test]
        public void ShouldNotCancelDeploymentOnTimeoutIfNotRequested()
        {
            Repository.Tasks
                .When(x => x.WaitForCompletion(Arg.Any<TaskResource[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Action<TaskResource[]>>()))
                .Do(x => { throw new TimeoutException(); });

            CommandLineArgs.Add("--project=" + ProjectName);
            CommandLineArgs.Add("--deploymenttimeout=00:00:01");
            CommandLineArgs.Add("--deployto=TestEnvironment");
            CommandLineArgs.Add("--version=latest");
            CommandLineArgs.Add("--progress");

            Assert.Throws<CommandException>(() => deployReleaseCommand.Execute(CommandLineArgs.ToArray()));

            Repository.Tasks.DidNotReceive().Cancel(Arg.Any<TaskResource>());
        }
    }
}
