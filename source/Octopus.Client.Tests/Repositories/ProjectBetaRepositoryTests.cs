using System;
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
    public class ProjectBetaRepositoryTests
    {
        const string MigrateLinkKey = "MigrateVariablesToGit";

        OctopusAsyncRepository repository;
        ConvertProjectVariablesToGitCommand commandUsed;
        string urlUsed;

        [SetUp]
        public void Setup()
        {
            var asyncClient = Substitute.For<IOctopusAsyncClient>();
            repository = new OctopusAsyncRepository(asyncClient);
            asyncClient.Post<ConvertProjectVariablesToGitCommand, ConvertProjectVariablesToGitResponse>(Arg.Do<string>(x => urlUsed = x), Arg.Do<ConvertProjectVariablesToGitCommand>(x => commandUsed = x)).Returns(new ConvertProjectVariablesToGitResponse());
        }

        [Test]
        public async Task MigrateVariablesToGit_CalledWithAllValues_PostsToClient()
        {
            // Arrange
            var migrateLink = "some/url";
            var project = new ProjectResource
            {
                Links = new LinkCollection
                {
                    {MigrateLinkKey, new Href(migrateLink)}
                }
            };

            // Act
            await repository.Projects.Beta().MigrateVariablesToGit(project, "branchy-branch", "Test commit message");

            // Assert
            urlUsed.Should().Be(migrateLink);
            commandUsed.Branch.Should().Be("branchy-branch");
            commandUsed.CommitMessage.Should().Be("Test commit message");
        }

        [Test]
        public void MigrateVariablesToGit_GitProjectWithVariablesAlreadyInGit_ThrowsError()
        {
            // Arrange
            var project = new ProjectResource
            {
                PersistenceSettings = new GitPersistenceSettingsResource
                {
                    ConversionState = new GitPersistenceSettingsConversionStateResource
                    {
                        VariablesAreInGit = true
                    }
                }
            };

            // Act + Assert
            repository.Projects.Beta().Awaiting(p => p.MigrateVariablesToGit(project, "branchy-branch", "Test commit message")).ShouldThrow<NotSupportedException>().WithMessage("*already been migrated*");
        }

        [Test]
        public void MigrateVariablesToGit_DoesNotHaveMigrateLink_ThrowsError()
        {
            // Arrange
            var project = new ProjectResource();

            // Act + Assert
            repository.Projects.Beta().Awaiting(p => p.MigrateVariablesToGit(project, "branchy-branch", "Test commit message")).ShouldThrow<NotSupportedException>().WithMessage("*not available*");
        }
    }
}