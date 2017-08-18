using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Client.Model;
using FluentAssertions;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class OverrideAutoDeployCommandFixture : ApiCommandFixtureBase
    {
        CreateAutoDeployOverrideCommand createAutoDeployOverrideCommand;

        private EnvironmentResource environment;
        private ProjectResource project;
        private ReleaseResource release;
        private TenantResource octopusTenant;

        private ProjectResource savedProject = default(ProjectResource);

        [SetUp]
        public void SetUp()
        {
            createAutoDeployOverrideCommand = new CreateAutoDeployOverrideCommand(RepositoryFactory, Log, FileSystem, ClientFactory, CommandOutputProvider);

            environment = new EnvironmentResource { Name = "Production", Id = "Environments-001" };
            project = new ProjectResource("Projects-1", "OctoFx", "OctoFx");
            release = new ReleaseResource("1.2.0", "Projects-1", "Channels-1");
            octopusTenant = new TenantResource
            {
                Id = "Tenants-1",
                Name = "Octopus",
                ProjectEnvironments = { ["Projects-1"] = new ReferenceCollection("Environments-001") }
            };

            Repository.Environments.FindByName("Production").Returns(
                environment
            );

            Repository.Projects.FindByName("OctoFx").Returns(
                project
            );

            Repository.Projects.GetReleaseByVersion(Arg.Any<ProjectResource>(), "1.2.0").Returns(
                release
            );

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

            Repository.Projects.When(x => x.Modify(Arg.Any<ProjectResource>()))
                .Do(x => savedProject = x.Args()[0] as ProjectResource);
        }

        [Test]
        public async Task ShouldAddOverrideForEnvironmentAndRelease()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");
            CommandLineArgs.Add("-version=1.2.0");

            await createAutoDeployOverrideCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("Auto deploy will deploy version 1.2.0 of the project OctoFx to the environment Production");
            await Repository.Projects.ReceivedWithAnyArgs().Modify(null).ConfigureAwait(false);
            var autoDeployOverride = savedProject.AutoDeployReleaseOverrides.Single();
            Assert.AreEqual(project.Id, savedProject.Id);
            Assert.AreEqual(release.Id, autoDeployOverride.ReleaseId);
            Assert.AreEqual(null, autoDeployOverride.TenantId);
            Assert.AreEqual(environment.Id, autoDeployOverride.EnvironmentId);
        }

        [Test]
        public async Task ShouldAddOverrideForTenantsByName()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");
            CommandLineArgs.Add("-version=1.2.0");
            CommandLineArgs.Add("-tenant=Octopus");

            await createAutoDeployOverrideCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("Auto deploy will deploy version 1.2.0 of the project OctoFx to the environment Production for the tenant Octopus");
            await Repository.Projects.ReceivedWithAnyArgs().Modify(null).ConfigureAwait(false);
            var autoDeployOverride = savedProject.AutoDeployReleaseOverrides.Single();
            Assert.AreEqual(project.Id, savedProject.Id);
            Assert.AreEqual(release.Id, autoDeployOverride.ReleaseId);
            Assert.AreEqual(octopusTenant.Id, autoDeployOverride.TenantId);
            Assert.AreEqual(environment.Id, autoDeployOverride.EnvironmentId);
        }

        [Test]
        public async Task ShouldAddOverrideForTenantsByTag()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");
            CommandLineArgs.Add("-version=1.2.0");
            CommandLineArgs.Add("-tenanttag=VIP");

            await createAutoDeployOverrideCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("Auto deploy will deploy version 1.2.0 of the project OctoFx to the environment Production for the tenant Octopus");
            await Repository.Projects.ReceivedWithAnyArgs().Modify(null).ConfigureAwait(false);
            var autoDeployOverride = savedProject.AutoDeployReleaseOverrides.Single();
            Assert.AreEqual(project.Id, savedProject.Id);
            Assert.AreEqual(release.Id, autoDeployOverride.ReleaseId);
            Assert.AreEqual(octopusTenant.Id, autoDeployOverride.TenantId);
            Assert.AreEqual(environment.Id, autoDeployOverride.EnvironmentId);
        }
    }
}