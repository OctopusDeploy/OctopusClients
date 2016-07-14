using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Client.Model;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class DeleteOverrideCommandFixture : ApiCommandFixtureBase
    {
        DeleteOverrideCommand deleteOverrideCommand;

        private EnvironmentResource environment;
        private ProjectResource project;
        private ReleaseResource release;
        private TenantResource octopusTenant;

        private ProjectResource savedProject = default(ProjectResource);

        [SetUp]
        public void SetUp()
        {
            deleteOverrideCommand = new DeleteOverrideCommand(RepositoryFactory, Log, FileSystem);

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
        public void ShouldDeleteOverrideForEnvironment()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");

            project.AutoDeployReleaseOverrides.Add(new AutoDeployReleaseOverrideResource(environment.Id, release.Id));

            deleteOverrideCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Deleted auto deploy release override for the project OctoFx to the environment Production");
            Repository.Projects.ReceivedWithAnyArgs().Modify(null);
            Assert.True(!savedProject.AutoDeployReleaseOverrides.Any());
        }

        [Test]
        public void ShouldDeleteOverrideForTenantByName()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");
            CommandLineArgs.Add("-tenant=Octopus");

            project.AutoDeployReleaseOverrides.Add(new AutoDeployReleaseOverrideResource(environment.Id, octopusTenant.Id, release.Id));

            deleteOverrideCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Deleted auto deploy release override for the project OctoFx to the environment Production and tenant Octopus");
            Repository.Projects.ReceivedWithAnyArgs().Modify(null);
            Assert.True(!savedProject.AutoDeployReleaseOverrides.Any());
        }

        [Test]
        public void ShouldDeleteOverrideForTenantByTag()
        {
            CommandLineArgs.Add("-project=OctoFx");
            CommandLineArgs.Add("-environment=Production");
            CommandLineArgs.Add("-tenanttag=VIP");

            project.AutoDeployReleaseOverrides.Add(new AutoDeployReleaseOverrideResource(environment.Id, octopusTenant.Id, release.Id));

            deleteOverrideCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Deleted auto deploy release override for the project OctoFx to the environment Production and tenant Octopus");
            Repository.Projects.ReceivedWithAnyArgs().Modify(null);
            Assert.True(!savedProject.AutoDeployReleaseOverrides.Any());
        }

    }
}