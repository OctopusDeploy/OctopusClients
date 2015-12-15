using NSubstitute;
using NUnit.Framework;
using OctopusTools.Commands;
using OctopusTools.Infrastructure;
using OctopusTools.Util;

namespace OctopusTools.Tests.Commands
{
    public class CreateReleaseCommandFixture : ApiCommandFixture
    {
        CreateReleaseCommand createReleaseCommand;
        IPackageVersionResolver versionResolver;

        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            versionResolver = Substitute.For<IPackageVersionResolver>();
        }

        [Test]
        public void ShouldLoadOptionsFromFile()
        {
            createReleaseCommand = new CreateReleaseCommand(RepositoryFactory, Log, new OctopusPhysicalFileSystem(Log), versionResolver);

            Assert.Throws<CouldNotFindException>(delegate {
                createReleaseCommand.Execute("--configfile=Commands/Resources/CreateRelease.config.txt");
            });
            
            Assert.AreEqual("Test Project", createReleaseCommand.ProjectName);
            Assert.AreEqual("1.0.0", createReleaseCommand.VersionNumber);
            Assert.AreEqual("Test config file.", createReleaseCommand.ReleaseNotes);
        }
    }
}