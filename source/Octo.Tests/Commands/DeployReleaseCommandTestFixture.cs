using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Model;
using Serilog;
using Serilog.Core;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class DeployReleaseCommandTestFixture : ApiCommandFixtureBase
    {
        DeployReleaseCommand deployReleaseCommand;
        const string ProjectName = "TestProject";
        TaskResource taskResource;

        [SetUp]
        public void SetUp()
        {
            deployReleaseCommand = new DeployReleaseCommand(RepositoryFactory, Log, FileSystem, ClientFactory);

            var project = new ProjectResource();
            var release = new ReleaseResource { Version = "1.0.0" };
            var releases = new ResourceCollection<ReleaseResource>(new[] { release }, new LinkCollection());
            var deploymentPromotionTarget = new DeploymentPromotionTarget { Name = "TestEnvironment", Id = "Env-1" };
            var promotionTargets = new List<DeploymentPromotionTarget> { deploymentPromotionTarget };
            var tenantPromotionTarget1 = new DeploymentPromomotionTenant() { Id = "Tenant-1", PromoteTo = promotionTargets };
            var tenantPromotionTarget2 = new DeploymentPromomotionTenant() { Id = "Tenant-2", PromoteTo = new List<DeploymentPromotionTarget>() };
            var deploymentTemplate = new DeploymentTemplateResource { PromoteTo = promotionTargets, TenantPromotions = { tenantPromotionTarget1, tenantPromotionTarget2 } };
            var deploymentPreviewResource = new DeploymentPreviewResource { StepsToExecute = new List<DeploymentTemplateStep>() };
            var deployment = new DeploymentResource { TaskId = "Task-1" };
            taskResource = new TaskResource() { Id = "Task-1" };

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
                .When(x => x.WaitForCompletion(Arg.Any<TaskResource[]>(), Arg.Any<int>(), TimeSpan.FromSeconds(1), Arg.Any<Func<TaskResource[], Task>>()))
                .Do(x => { throw new TimeoutException(); });

            CommandLineArgs.Add("--project=" + ProjectName);
            CommandLineArgs.Add("--deploymenttimeout=00:00:01");
            CommandLineArgs.Add("--deployto=TestEnvironment");
            CommandLineArgs.Add("--version=latest");
            CommandLineArgs.Add("--progress");
            CommandLineArgs.Add("--cancelontimeout");

            Func<Task> exec = () => deployReleaseCommand.Execute(CommandLineArgs.ToArray());
            exec.ShouldThrow<CommandException>();

            Repository.Tasks.Received().Cancel(taskResource).GetAwaiter().GetResult();
        }

        [Test]
        public void ShouldNotCancelDeploymentOnTimeoutIfNotRequested()
        {
            Repository.Tasks
                .When(x => x.WaitForCompletion(Arg.Any<TaskResource[]>(), Arg.Any<int>(), TimeSpan.FromSeconds(1), Arg.Any<Func<TaskResource[], Task>>()))
                .Do(x =>{ throw new TimeoutException(); });

            CommandLineArgs.Add("--project=" + ProjectName);
            CommandLineArgs.Add("--deploymenttimeout=00:00:01");
            CommandLineArgs.Add("--deployto=TestEnvironment");
            CommandLineArgs.Add("--version=latest");
            CommandLineArgs.Add("--progress");

            Func<Task> exec = () => deployReleaseCommand.Execute(CommandLineArgs.ToArray());
            exec.ShouldThrow<CommandException>();
            
            Repository.Tasks.DidNotReceive().Cancel(Arg.Any<TaskResource>());
        }


        [Test]
        public void ShouldRejectIfMultipleEnvironmentsAndTenanted()
        {
            CommandLineArgs.Add("--project=" + ProjectName);
            CommandLineArgs.Add("--deploymenttimeout=00:00:01");
            CommandLineArgs.Add("--deployto=TestEnvironment");
            CommandLineArgs.Add("--deployto=DevEnvironment");
            CommandLineArgs.Add("--tenant=James");
            CommandLineArgs.Add("--version=latest");
            CommandLineArgs.Add("--progress");
            CommandLineArgs.Add("--cancelontimeout");


            Func<Task> exec = () => deployReleaseCommand.Execute(CommandLineArgs.ToArray());
            exec.ShouldThrow<CommandException>()
                .WithMessage("Please specify only one environment at a time when deploying to tenants.");

            Repository.Deployments.DidNotReceive().Create(Arg.Any<DeploymentResource>());
        }


        [Test]
        public void ShouldTryLoadTenant()
        {
            var tenant1 = new TenantResource() { Id = "Tenant-1", Name = "Tenant One" };
            Repository.Tenants
                .Get(Arg.Is<string[]>(t => t.SequenceEqual(new[] { "Tenant-1" })))
                .Returns(new List<TenantResource>() { tenant1 });
            taskResource.State = TaskState.Success;

            CommandLineArgs.Add("--project=" + ProjectName);
            CommandLineArgs.Add("--deploymenttimeout=00:00:01");
            CommandLineArgs.Add("--deployto=TestEnvironment");
            CommandLineArgs.Add("--tenant=*");
            CommandLineArgs.Add("--version=latest");
            CommandLineArgs.Add("--progress");
            CommandLineArgs.Add("--cancelontimeout");

            deployReleaseCommand.Execute(CommandLineArgs.ToArray()).GetAwaiter().GetResult();

            Repository.Deployments.Received(1).Create(Arg.Any<DeploymentResource>());
            Repository.Deployments.Received()
                .Create(Arg.Is<DeploymentResource>(dr => dr.TenantId == tenant1.Id && dr.EnvironmentId == "Env-1"));
        }
    }
}
