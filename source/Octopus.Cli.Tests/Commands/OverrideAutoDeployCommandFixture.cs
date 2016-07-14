using System.Collections.Generic;
using NSubstitute;
using NSubstitute.Routing.AutoValues;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Client.Model;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class OverrideAutoDeployCommandFixture : ApiCommandFixtureBase
    {
        OverrideAutoDeployCommand overrideAutoDeployCommand;

        [SetUp]
        public void SetUp()
        {
            overrideAutoDeployCommand = new OverrideAutoDeployCommand(RepositoryFactory, Log, FileSystem);

            Repository.Environments.FindByName("Production").Returns(
                new EnvironmentResource { Name = "Production", Id = "Environments-001" }
            );

            Repository.Projects.FindByName("OctoFx").Returns(
                new ProjectResource("Projects-1", "OctoFx", "OctoFx")
            );

            Repository.Projects.GetReleaseByVersion(Arg.Any<ProjectResource>(), "1.2.0").Returns(
                new ReleaseResource("1.2.0", "Projects-1", "Channels-1")
            );

            var octopusTenant = new TenantResource
            {
                Name = "Octopus",
                ProjectEnvironments = { ["Projects-1"] = new ReferenceCollection("Environments-001") }
            };
            Repository.Tenants.FindByNames(Arg.Any<IEnumerable<string>>()).Returns(
                new List<TenantResource>
                {
                    octopusTenant
                }
            );
            Repository.Tenants.FindAll(null, Arg.Any<string[]>()).Returns(
                new List<TenantResource>
                {
                    octopusTenant
                }
            );
        }

        [Test]
        public void ShouldAddOverrideForEnvironmentAndRelease()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");
            CommandLineArgs.Add("-version=1.2.0");

            overrideAutoDeployCommand.Execute(CommandLineArgs.ToArray());
            Log.Received().Info("Auto deploy will deploy version 1.2.0 of the project OctoFx to the environment Production");
            Repository.Projects.ReceivedWithAnyArgs().Modify(null);
        }

        [Test]
        public void ShouldAddOverrideForTenantsByName()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");
            CommandLineArgs.Add("-version=1.2.0");
            CommandLineArgs.Add("-tenant=Octopus");

            overrideAutoDeployCommand.Execute(CommandLineArgs.ToArray());
            Log.Received().Info("Auto deploy will deploy version 1.2.0 of the project OctoFx to the environment Production for the tenant Octopus");
            Repository.Projects.ReceivedWithAnyArgs().Modify(null);
        }

        [Test]
        public void ShouldAddOverrideForTenantsByTag()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");
            CommandLineArgs.Add("-version=1.2.0");
            CommandLineArgs.Add("-tenanttag=VIP");

            overrideAutoDeployCommand.Execute(CommandLineArgs.ToArray());
            Log.Received().Info("Auto deploy will deploy version 1.2.0 of the project OctoFx to the environment Production for the tenant Octopus");
            Repository.Projects.ReceivedWithAnyArgs().Modify(null);
        }
    }
}