using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Repositories
{
    [TestFixture]
    public class VariableSetBetaRepositoryTests
    {
        const string VariablesLinkKey = "Variables";
        const string SensitiveVariablesLinkKey = "SensitiveVariables";

        OctopusAsyncRepository repository;

        string getUrlUsed;
        object getParamsUsed;

        [SetUp]
        public void Setup()
        {
            var asyncClient = Substitute.For<IOctopusAsyncClient>();
            repository = new OctopusAsyncRepository(asyncClient);

            asyncClient.Get<VariableSetResource>(Arg.Do<string>(x => getUrlUsed = x), Arg.Any<CancellationToken>(), Arg.Do<object>(x => getParamsUsed = x));
        }

        [Test]
        public async Task Get_ProjectHasGitVariablesAndPassedGitRef_UsesVariablesUrl()
        {
            // Arrange
            var variableLink = "some/url";
            var project = TestProject(true, true, VariablesLinkKey, variableLink);

            // Act
            await repository.VariableSets.Get(project, "branchy");

            // Assert
            getUrlUsed.Should().Be(variableLink);
            getParamsUsed.ShouldBeEquivalentTo(new {gitRef = "branchy"});
        }


        [Test]
        public async Task Get_ProjectHasGitVariablesButNotPassedGitRef_UsesSensitiveVariablesUrl()
        {
            // Arrange
            var variableLink = "some/url";
            var project = TestProject(true, true, SensitiveVariablesLinkKey, variableLink);

            // Act
            await repository.VariableSets.Get(project);

            // Assert
            getUrlUsed.Should().Be(variableLink);
            getParamsUsed.Should().BeNull();
        }

        [Test]
        public async Task Get_ProjectDoesNotHaveGitVariables_UsesVariablesUrl()
        {
            // Arrange
            var variableLink = "some/url";
            var project = TestProject(true, false, VariablesLinkKey, variableLink);

            // Act
            await repository.VariableSets.Get(project);

            // Assert
            getUrlUsed.Should().Be(variableLink);
            getParamsUsed.Should().BeNull();
        }

        [Test]
        public async Task Get_ProjectIsInTheDatabase_UsesVariablesUrl()
        {
            // Arrange
            var variableLink = "some/url";
            var project = TestProject(false, false, VariablesLinkKey, variableLink);

            // Act
            await repository.VariableSets.Get(project);

            // Assert
            getUrlUsed.Should().Be(variableLink);
            getParamsUsed.Should().BeNull();
        }


        ProjectResource TestProject(bool gitProject, bool variablesAreInGit, string linkKey, string link)
        {
            var linkCollection = new LinkCollection {{linkKey, link}};

            PersistenceSettingsResource persistence = new DatabasePersistenceSettingsResource();


            if (gitProject)
            {
                persistence = new GitPersistenceSettingsResource
                {
                    ConversionState = new GitPersistenceSettingsConversionStateResource
                    {
                        VariablesAreInGit = variablesAreInGit
                    }
                };
            }

            return new ProjectResource
            {
                PersistenceSettings = persistence,
                Links = linkCollection
            };
        }
    }
}