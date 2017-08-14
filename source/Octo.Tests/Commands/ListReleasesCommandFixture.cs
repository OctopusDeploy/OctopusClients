using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Client.Model;
using FluentAssertions;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class ListReleasesCommandFixture : ApiCommandFixtureBase
    {
        ListReleasesCommand listReleasesCommand;

        [SetUp]
        public void SetUp()
        {
            listReleasesCommand = new ListReleasesCommand(RepositoryFactory, Log, FileSystem, ClientFactory, CommandOutputProvider);
        }

        [Test]
        public async Task ShouldGetListOfReleases()
        {
            Repository.Projects.FindByNames(Arg.Any<IEnumerable<string>>()).Returns(new List<ProjectResource>
                {
                    new ProjectResource {Name = "ProjectA", Id = "projectaid"},
                    new ProjectResource {Name = "ProjectB", Id = "projectbid"}
                });

            Repository.Releases.FindMany(Arg.Any<Func<ReleaseResource, bool>>()).Returns(new List<ReleaseResource>
                {
                    new ReleaseResource {ProjectId = "projectaid", Version="1.0", Assembled = DateTimeOffset.MinValue, ReleaseNotes = "Release Notes 1"},
                    new ReleaseResource {ProjectId = "projectaid", Version="2.0", Assembled = DateTimeOffset.MaxValue, ReleaseNotes = "Release Notes 2"}
                });

            CommandLineArgs.Add("--project=ProjectA");

            await listReleasesCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain(string.Format("[Information] Releases: {0}", 2));
            LogLines.Should().Contain(string.Format("[Information]  - Project: {0}", "ProjectA"));
            LogLines.Should().Contain(string.Format("[Information]     {0}", "Version: 1.0"));
            LogLines.Should().Contain(string.Format("[Information]     {0}", "Assembled: " + DateTimeOffset.MinValue));
            LogLines.Should().Contain(string.Format("[Information]     {0}", "Package Versions: "));
            LogLines.Should().Contain(string.Format("[Information]     {0}", "Release Notes: Release Notes 1"));
            LogLines.Should().Contain(string.Format("[Information]     {0}", "Version: 2.0"));
            LogLines.Should().Contain(string.Format("[Information]     {0}", "Assembled: " + DateTimeOffset.MaxValue));
            LogLines.Should().Contain(string.Format("[Information]     {0}", "Package Versions: "));
            LogLines.Should().Contain(string.Format("[Information]     {0}", "Release Notes: Release Notes 2"));
        }
    }
}
