using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands.Deployment;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octo.Tests.Commands
{
    [TestFixture]
    public class ListDeploymentsCommandFixture : ApiCommandFixtureBase
    {
        ListDeploymentsCommand listDeploymentsCommands;

        [SetUp]
        public void SetUp()
        {
            listDeploymentsCommands = new ListDeploymentsCommand(RepositoryFactory, FileSystem, ClientFactory, CommandOutputProvider);

            var deploymentResources = new ResourceCollection<DeploymentResource>(
                new List<DeploymentResource>
                {
                    new DeploymentResource
                    {
                        Name = "",
                        Id = "deploymentid1",
                        ProjectId = "projectaid",
                        EnvironmentId = "environmentid1"
                    },
                    new DeploymentResource
                    {
                        Name = "",
                        Id = "deploymentid2",
                        ProjectId = "projectbid",
                        EnvironmentId = "environmentid2"
                    },
                }, new LinkCollection());

            Repository.Tenants.Status()
                .ReturnsForAnyArgs(new MultiTenancyStatusResource());

            Repository.Deployments
                .When(
                    x =>
                        x.Paginate(Arg.Any<string[]>(), Arg.Any<string[]>(), Arg.Any<string[]>(),
                            Arg.Any<Func<ResourceCollection<DeploymentResource>, bool>>()))
                .Do(r => r.Arg<Func<ResourceCollection<DeploymentResource>, bool>>()(deploymentResources));

            Repository.Projects.FindAll()
                .Returns(Task.FromResult(
                    new List<ProjectResource>
                    {
                        new ProjectResource {Name = "ProjectA", Id = "projectaid"},
                        new ProjectResource {Name = "ProjectB", Id = "projectbid"},
                    }));

            Repository.Environments.FindAll()
                .Returns(Task.FromResult(
                    new List<EnvironmentResource>
                    {
                        new EnvironmentResource {Name = "EnvA", Id = "environmentid1"},
                        new EnvironmentResource {Name = "EnvB", Id = "environmentid2"}
                    }));

            Repository.Tenants.FindAll()
                .Returns(Task.FromResult(new List<TenantResource>()));

            Repository.Releases.Get(Arg.Any<string>()).ReturnsForAnyArgs(new ReleaseResource { Version = "0.0.1" });
        }

        [Test]
        public async Task ShouldGetListOfDeployments()
        {
            var argsWithNumber = new List<string>(CommandLineArgs)
            {
                "--number=1"
            };

            await listDeploymentsCommands.Execute(argsWithNumber.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain(" - Project: ProjectA");
            LogLines.Should().NotContain(" - Project: ProjectB");
        }

        [Test]
        public async Task JsonOutput_ShouldBeWellFormedJson()
        {
            CommandLineArgs.Add("--outputFormat=json");

            await listDeploymentsCommands.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            var logoutput = LogOutput.ToString();
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain("ProjectA");
            logoutput.Should().Contain("ProjectB");
        }
    }
}