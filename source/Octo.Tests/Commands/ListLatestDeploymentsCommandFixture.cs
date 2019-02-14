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
    public class ListLatestDeploymentsCommandFixture : ApiCommandFixtureBase
    {
        ListLatestDeploymentsCommand listLatestDeploymentsCommands;

        [SetUp]
        public void SetUp()
        {
            listLatestDeploymentsCommands = new ListLatestDeploymentsCommand(RepositoryFactory, FileSystem, ClientFactory, CommandOutputProvider);

            var dashboardResources = new DashboardResource
            {
                Items = new List<DashboardItemResource>
                {
                    new DashboardItemResource
                    {
                        EnvironmentId = "environmentid1",
                        ProjectId = "projectaid",
                        TenantId = "tenantid1"                        
                    },
                    new DashboardItemResource
                    {
                        EnvironmentId = "environmentid1",
                        ProjectId = "projectaid",
                        TenantId = "tenantid2"                        
                    }
                },
                Tenants = new List<DashboardTenantResource>
                {
                    new DashboardTenantResource
                    {
                        Id = "tenantid1",
                        Name = "tenant1"
                    }
                }
            };

            Repository.Projects.FindByNames(Arg.Any<IEnumerable<string>>())
                .Returns(Task.FromResult(
                    new List<ProjectResource>
                    {
                        new ProjectResource {Name = "ProjectA", Id = "projectaid"}
                    }));

            Repository.Environments.FindAll()
                .Returns(Task.FromResult(
                    new List<EnvironmentResource>
                    {
                        new EnvironmentResource {Name = "EnvA", Id = "environmentid1"}
                    }));

            Repository.Releases.Get(Arg.Any<string>()).ReturnsForAnyArgs(new ReleaseResource { Version = "0.0.1" });

            Repository.Dashboards.GetDynamicDashboard(Arg.Any<string[]>(), Arg.Any<string[]>()).ReturnsForAnyArgs(dashboardResources);
        }

        [Test]
        public async Task ShouldNotFailWhenTenantIsRemoved()
        {
            CommandLineArgs.Add("--project=ProjectA");

            await listLatestDeploymentsCommands.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain(" - Tenant: tenant1");
            LogLines.Should().Contain(" - Tenant: <Removed>");
        }

        [Test]
        public async Task JsonOutput_ShouldNotFailOnRemovedTenant()
        {
            CommandLineArgs.Add("--project=ProjectA");
            CommandLineArgs.Add("--outputFormat=json");

            await listLatestDeploymentsCommands.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            var logoutput = LogOutput.ToString();
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain("tenant1");
            logoutput.Should().Contain("<Removed>");
        }
    }
}