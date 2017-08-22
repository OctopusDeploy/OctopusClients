using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Commands.Releases;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Tests.Helpers;
using Octopus.Cli.Util;

namespace Octopus.Cli.Tests.Commands
{
    public class CreateReleaseCommandFixture : ApiCommandFixtureBase
    {
        CreateReleaseCommand createReleaseCommand;
        IPackageVersionResolver versionResolver;
        IReleasePlanBuilder releasePlanBuilder;

        [SetUp]
        public void SetUp()
        {
            versionResolver = Substitute.For<IPackageVersionResolver>();
            releasePlanBuilder = Substitute.For<IReleasePlanBuilder>();
        }

        [Test]
        public void ShouldLoadOptionsFromFile()
        {
            createReleaseCommand = new CreateReleaseCommand(RepositoryFactory, Log, new OctopusPhysicalFileSystem(Log), versionResolver, releasePlanBuilder, ClientFactory, CommandOutputProvider);

            Assert.Throws<CouldNotFindException>(delegate {
                createReleaseCommand.Execute("--configfile=Commands/Resources/CreateRelease.config.txt");
            });
            
            Assert.AreEqual("Test Project", createReleaseCommand.ProjectName);
            Assert.AreEqual("1.0.0", createReleaseCommand.VersionNumber);
            Assert.AreEqual("Test config file.", createReleaseCommand.ReleaseNotes);
        }
    }
}