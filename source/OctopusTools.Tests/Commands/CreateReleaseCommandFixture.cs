using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;
using OctopusTools.Commands;
using OctopusTools.Infrastructure;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class CreateReleaseCommandFixture : ApiCommandFixtureBase
    {

        CreateReleaseCommand command;

        [SetUp]
        public void Setup()
        {
            command = new CreateReleaseCommand(RepositoryFactory, Log, new PackageVersionResolver(Log));

            //fake project
            Repository.Projects.FindByName("ProjectA").Returns(info => new ProjectResource("projecta", "ProjectA", "a") {DeploymentProcessId = "deploymentprocess1"});

            //fake deployment process
            Repository.DeploymentProcesses.Get("deploymentprocess1").Returns(new DeploymentProcessResource()
            {
                Links = new LinkCollection() { {"Template", "footemplate"} }
            });

            //fake template
            Repository.DeploymentProcesses.GetTemplate(Arg.Any<DeploymentProcessResource>()).Returns(new ReleaseTemplateResource()
            {
                NextVersionIncrement = "1.0",
                Packages = new List<ReleaseTemplatePackage>()
                {
                    new ReleaseTemplatePackage(){NuGetFeedId = "feed1", IsResolvable = true, NuGetPackageId = "Package1", StepName = "StepA"}
                }
            });

            //fake feed
            Repository.Feeds.Get("feed1").Returns(new FeedResource()
            {
                FeedUri = "foo://",
                Links = new LinkCollection()
                {
                    {"VersionsTemplate", "feed1versions"},
                    {"SearchTemplate", "feed1search"}
                }
            });


        }

        [Test]
        public void when_project_not_exists_should_throw_exception()
        {
            command.ProjectName = "ProjectB";
            Assert.Throws<CommandException>(()=>command.Execute(CommandLineArgs.ToArray()), "Could not find a project named: ProjectB");
        }

        [Test]
        [ExpectedException(typeof(CommandException))]
        public void when_no_versions_should_log_error()
        {
            command.ProjectName = "ProjectA";
            Repository.Client.Get<List<PackageResource>>("feed1versions", Arg.Any<object>()).Returns(new List<PackageResource>());
            command.Execute(CommandLineArgs.ToArray());

            Log.Received().ErrorFormat("Could not find any packages with ID '{0}' in the feed '{1}'", "Package1", "foo://");

        }

        [Test]
        public void when_no_versions_should_throw_exception()
        {
            command.ProjectName = "ProjectA";
            Repository.Client.Get<List<PackageResource>>("feed1versions", Arg.Any<object>()).Returns(new List<PackageResource>());

            Assert.Throws<CommandException>(()=>command.Execute(CommandLineArgs.ToArray()));
        }

        [Test]
        public void when_version_exists_should_create_release()
        {
            command.ProjectName = "ProjectA";
            Repository.Client.Get<List<PackageResource>>("feed1versions", Arg.Any<object>()).Returns(new List<PackageResource>()
            {
                new PackageResource(){NuGetPackageId = "Package1", Version = "1.0.0"},
            });
            Repository.Releases.Create(Arg.Any<ReleaseResource>()).Returns(new ReleaseResource());
            command.Execute(CommandLineArgs.ToArray());

            Repository.Releases.Received().Create(Arg.Any<ReleaseResource>());

        }

        [Test]
        public void when_version_exists_should_include_in_release()
        {
            command.ProjectName = "ProjectA";
            Repository.Client.Get<List<PackageResource>>("feed1versions", Arg.Any<object>()).Returns(new List<PackageResource>()
            {
                new PackageResource(){NuGetPackageId = "Package1", Version = "1.0.0"},
            });

            Repository.Releases.Create(Arg.Any<ReleaseResource>()).Returns(new ReleaseResource());
            command.Execute(CommandLineArgs.ToArray());

            Repository.Releases.Received().Create(Arg.Is<ReleaseResource>(r=>r.SelectedPackages.Any(p=>p.Version.Equals("1.0.0"))));

        }
        
        [Test]
        public void when_prerelease_version_specified_should_use_that()
        {
            command.ProjectName = "ProjectA";
            command.VersionPrerelease = "Dev";
            Repository.Client.Get<List<PackageResource>>("feed1search", Arg.Any<object>()).Returns(new List<PackageResource>()
            {
                new PackageResource(){NuGetPackageId = "Package1", Version = "1.0.0"},
                new PackageResource(){NuGetPackageId = "Package1", Version = "1.0.0-Dev"},
            });

            Repository.Releases.Create(Arg.Any<ReleaseResource>()).Returns(new ReleaseResource());
            command.Execute(CommandLineArgs.ToArray());

            Repository.Releases.Received().Create(Arg.Is<ReleaseResource>(r => r.SelectedPackages.Any(p => p.Version.Equals("1.0.0-Dev"))));
        }


    }
}
