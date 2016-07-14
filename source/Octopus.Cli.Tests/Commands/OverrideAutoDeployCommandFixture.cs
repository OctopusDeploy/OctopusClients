using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Client.Model;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class OverrideAutoDeployCommandFixture : ApiCommandFixtureBase
    {
        OverrideAutoDeployCommand overrideAutoDeployCommand;

        readonly EnvironmentResource environment = new EnvironmentResource {Name = "Production", Id = "Environments-001"};
        readonly ProjectResource project = new ProjectResource("Projects-1", "OctoFx", "OctoFx");
        readonly ReleaseResource release = new ReleaseResource("1.2.0", "Projects-1", "Channels-1");
        readonly TenantResource octopusTenant = new TenantResource
        {
            Name = "Octopus",
            ProjectEnvironments = { ["Projects-1"] = new ReferenceCollection("Environments-001") }
        };

        [SetUp]
        public void SetUp()
        {
            overrideAutoDeployCommand = new OverrideAutoDeployCommand(RepositoryFactory, Log, FileSystem);

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
        }

        [Test]
        public void ShouldAddOverrideForEnvironmentAndRelease()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");
            CommandLineArgs.Add("-version=1.2.0");

            var savedProject = default(ProjectResource);
            Repository.Projects.When(x => x.Modify(Arg.Any<ProjectResource>()))
                .Do(x => savedProject = x.Args()[0] as ProjectResource);

            overrideAutoDeployCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Auto deploy will deploy version 1.2.0 of the project OctoFx to the environment Production");
            Repository.Projects.ReceivedWithAnyArgs().Modify(null);
            var autoDeployOverride = savedProject.AutoDeployReleaseOverrides.Single();
            Assert.AreEqual(project.Id, savedProject.Id);
            Assert.AreEqual(release.Id, autoDeployOverride.ReleaseId);
            Assert.AreEqual(null, autoDeployOverride.TenantId);
            Assert.AreEqual(environment.Id, autoDeployOverride.EnvironmentId);
        }

        [Test]
        public void ShouldAddOverrideForTenantsByName()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");
            CommandLineArgs.Add("-version=1.2.0");
            CommandLineArgs.Add("-tenant=Octopus");

            var savedProject = default(ProjectResource);
            Repository.Projects.When(x => x.Modify(Arg.Any<ProjectResource>()))
                .Do(x => savedProject = x.Args()[0] as ProjectResource);


            overrideAutoDeployCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Auto deploy will deploy version 1.2.0 of the project OctoFx to the environment Production for the tenant Octopus");
            Repository.Projects.ReceivedWithAnyArgs().Modify(null);
            var autoDeployOverride = savedProject.AutoDeployReleaseOverrides.Single();
            Assert.AreEqual(project.Id, savedProject.Id);
            Assert.AreEqual(release.Id, autoDeployOverride.ReleaseId);
            Assert.AreEqual(octopusTenant.Id, autoDeployOverride.TenantId);
            Assert.AreEqual(environment.Id, autoDeployOverride.EnvironmentId);
        }

        [Test]
        public void ShouldAddOverrideForTenantsByTag()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");
            CommandLineArgs.Add("-version=1.2.0");
            CommandLineArgs.Add("-tenanttag=VIP");

            var savedProject = default(ProjectResource);
            Repository.Projects.When(x => x.Modify(Arg.Any<ProjectResource>()))
                .Do(x => savedProject = x.Args()[0] as ProjectResource);


            overrideAutoDeployCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Auto deploy will deploy version 1.2.0 of the project OctoFx to the environment Production for the tenant Octopus");
            Repository.Projects.ReceivedWithAnyArgs().Modify(null);
            var autoDeployOverride = savedProject.AutoDeployReleaseOverrides.Single();
            Assert.AreEqual(project.Id, savedProject.Id);
            Assert.AreEqual(release.Id, autoDeployOverride.ReleaseId);
            Assert.AreEqual(octopusTenant.Id, autoDeployOverride.TenantId);
            Assert.AreEqual(environment.Id, autoDeployOverride.EnvironmentId);
        }
    }
}