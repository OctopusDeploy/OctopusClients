using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Model.Git;

namespace Octopus.Client.Tests.Repositories
{
    [TestFixture]
    public class ProjectRepositoryFixture
    {
        OctopusAsyncRepository asyncRepository;
        string asyncUrlUsed;

        OctopusRepository syncRepository;
        string syncUrlUsed;

        [SetUp]
        public void Setup()
        {
            var asyncClient = Substitute.For<IOctopusAsyncClient>();
            asyncRepository = new OctopusAsyncRepository(asyncClient);
            asyncClient.Post<ConvertProjectToGitCommand, ConvertProjectToGitResponse>(Arg.Do<string>(x => asyncUrlUsed = x), Arg.Any<ConvertProjectToGitCommand>(), Arg.Any<CancellationToken>()).Returns(new ConvertProjectToGitResponse());

            var syncClient = Substitute.For<IOctopusClient>();
            syncRepository = new OctopusRepository(syncClient);
            syncClient.Post<ConvertProjectToGitCommand, ConvertProjectToGitResponse>(Arg.Do<string>(x => syncUrlUsed = x), Arg.Any<ConvertProjectToGitCommand>()).Returns(new ConvertProjectToGitResponse());
        }

        [Test]
        public async Task ConvertToGit_HasConvertToGitAndConvertToVcsLink_UsesConvertToGitLink()
        {
            // Arrange
            var gitUrl = "/convert/to/git";
            var project = new ProjectResource
            {
                Links = new LinkCollection
                {
                    {"ConvertToGit", new Href(gitUrl)},
                    {"ConvertToVcs", new Href("/convert/to/vcs")}
                }
            };

            // Act
            await asyncRepository.Projects.ConvertToGit(project, new GitPersistenceSettingsResource(), string.Empty);
            syncRepository.Projects.ConvertToGit(project, new GitPersistenceSettingsResource(), string.Empty);

            // Assert
            asyncUrlUsed.Should().Be(gitUrl);
            syncUrlUsed.Should().Be(gitUrl);
        }

        [Test]
        public async Task ConvertToGit_OnlyHasConvertToVcsLink_UsesConvertToVcsLink()
        {
            // Arrange
            var vcsUrl = "/convert/to/vcs";
            var project = new ProjectResource
            {
                Links = new LinkCollection { { "ConvertToVcs", new Href(vcsUrl) } }
            };

            // Act
            await asyncRepository.Projects.ConvertToGit(project, new GitPersistenceSettingsResource(), string.Empty);
            syncRepository.Projects.ConvertToGit(project, new GitPersistenceSettingsResource(), string.Empty);

            // Assert
            asyncUrlUsed.Should().Be(vcsUrl);
            syncUrlUsed.Should().Be(vcsUrl);
        }

        [Test]
        public async Task ConvertToGit_OnlyHasConvertToGitLink_UsesConvertToGitLink()
        {
            // Arrange
            var gitUrl = "/convert/to/git";
            var project = new ProjectResource
            {
                Links = new LinkCollection { { "ConvertToGit", new Href(gitUrl) } }
            };

            // Act
            await asyncRepository.Projects.ConvertToGit(project, new GitPersistenceSettingsResource(), string.Empty);
            syncRepository.Projects.ConvertToGit(project, new GitPersistenceSettingsResource(), string.Empty);

            // Assert
            asyncUrlUsed.Should().Be(gitUrl);
            syncUrlUsed.Should().Be(gitUrl);
        }
    }
}