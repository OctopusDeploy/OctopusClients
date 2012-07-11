using log4net;
using NSubstitute;
using NUnit.Framework;
using OctopusTools.Client;
using OctopusTools.Commands;
using OctopusTools.Model;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class CreateReleaseCommandFixture
    {
        private CreateReleaseCommand createCommand;

        [SetUp]
        public void SetUp()
        {
            var sessionFactory = Substitute.For<IOctopusSessionFactory>();
            var log = Substitute.For<ILog>();
            var deploymentWatcher = Substitute.For<IDeploymentWatcher>();
            createCommand = new CreateReleaseCommand(sessionFactory, log, deploymentWatcher);
        }


        [Test]
        public void ShouldCreateDictionaryUponParsingVersionConstraints()
        {
            createCommand.ParsePackageConstraint("packageIdA:1.0.0");
            createCommand.ParsePackageConstraint("packageIdB:2.0.0");
            createCommand.ParsePackageConstraint("packageIdC:1.0.0");

            Assert.IsTrue(createCommand.PackageVersionNumberOverrides.ContainsKey("packageIdA"));
            Assert.IsTrue(createCommand.PackageVersionNumberOverrides.ContainsKey("packageIdB"));
            Assert.IsTrue(createCommand.PackageVersionNumberOverrides.ContainsKey("packageIdC"));

            Assert.That(createCommand.PackageVersionNumberOverrides["packageIdA"].Equals("1.0.0"));
            Assert.That(createCommand.PackageVersionNumberOverrides["packageIdB"].Equals("2.0.0"));
            Assert.That(createCommand.PackageVersionNumberOverrides["packageIdC"].Equals("1.0.0"));
        }

        [Test]
        public void ShouldLetPackageSpecificVersionConstraintOverridePackageVersion()
        {
            createCommand.PackageVersionNumber = "4.2.1";
            createCommand.ParsePackageConstraint("packageA:1.0.0");
            Step step = Substitute.For<Step>();
            step.NuGetPackageId = "packageA";

            Assert.AreEqual("1.0.0", createCommand.GetPackageVersionForStep(step));
        }
    }
}
