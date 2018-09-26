using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Tests.Spaces
{
    [TestFixture]
    public class SpaceIdAsyncTests
    {

        IOctopusAsyncClient SetupAsyncClient(string spaceId)
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            client.IsAuthenticated.Returns(true);
            client.Get<UserResource>(Arg.Any<string>()).Returns(new UserResource() { Links = { { "Spaces", "" } } });
            client.Get<SpaceResource[]>(Arg.Any<string>()).Returns(new[] { new SpaceResource() { Id = spaceId, IsDefault = true } });
            client.Get<SpaceRootResource>(Arg.Any<string>(), Arg.Any<object>()).Returns(new SpaceRootResource());
            client.Get<RootResource>(Arg.Any<string>()).Returns(new RootResource()
            {
                ApiVersion = "3.0.0",
                Links =
                {
                    { "Teams", "" },
                    { "ProjectGroups", "" },
                    { "CurrentUser",  ""},
                    { "SpaceHome",  ""},
                }
            });
            return client;
        }

        [Test]
        [TestCase("Spaces-1", TestName = "Space1")]
        [TestCase("Spaces-2", TestName = "Space2")]
        public void MixedScoped_SingleSpaceContextShouldEnrichSpaceId(string spaceId)
        {
            var client = SetupAsyncClient(spaceId);
            client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t =>
            {
                t.SpaceId.Should().Be(spaceId);
            }));
            var teamRepo = new TeamsRepository(OctopusAsyncRepository.Create(client, SpaceContext.SpecificSpace(spaceId)).Result);
            var created = teamRepo.Create(new TeamResource() { Name = "Test" }).Result;
        }

        [Test]
        [TestCase("Spaces-1", TestName = "Space1")]
        [TestCase("Spaces-2", TestName = "Space2")]
        public void SpaceScoped_SingleSpaceContextShouldNotEnrichSpaceId(string spaceId)
        {
            var client = SetupAsyncClient(spaceId);
            client.Create(Arg.Any<string>(), Arg.Do<ProjectGroupResource>(t =>
            {
                t.SpaceId.Should().BeNullOrEmpty();
            }));
            var repo = new ProjectGroupRepository(OctopusAsyncRepository.Create(client, SpaceContext.SpecificSpace(spaceId)).Result);
            var _ = repo.Create(new ProjectGroupResource { Name = "Test" }).Result;
        }

        [Test]
        [TestCase("Spaces-2", false, TestName = "Spacific spaces")]
        [TestCase("Spaces-2", true, TestName = "Specific space with system")]
        [TestCase("Spaces-2", true, TestName = "Specific spaces with system")]
        [TestCase(null, true, TestName = "System only")]
        public void NonSingleSpaceContextShouldNotEnrichSpaceId(string includingSpaceId, bool includeSystem)
        {
            var client = SetupAsyncClient("Spaces-1");
            client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t =>
            {
                t.SpaceId.Should().BeNullOrEmpty();
            }));
            var includingSpaceContext = new SpaceContext(new []{includingSpaceId}, includeSystem);
            var teamRepo = new TeamsRepository(OctopusAsyncRepository.Create(client).Result);
            var multiScoped = teamRepo.Including(includingSpaceContext);
            var _ = multiScoped.Create(new TeamResource() { Name = "Test" }).Result;
        }

        [Test]
        [TestCase("Spaces-2", false, TestName = "Spacific spaces")]
        [TestCase("Spaces-1", true, TestName = "Specific space with system")]
        [TestCase("Spaces-2", true, TestName = "Specific spaces with system")]
        public void NonSingleSpaceContextShouldNotChangeExistingSpaceId(string includingSpaceId, bool includeSystem)
        {
            var client = SetupAsyncClient("Spaces-1");
            client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t => t.SpaceId.Should().BeNullOrEmpty()));
            var teamRepo = new TeamsRepository(OctopusAsyncRepository.Create(client).Result);
            var multiScoped = teamRepo.Including(new SpaceContext(new[] {includingSpaceId}, includeSystem));
            var _ = multiScoped.Create(new TeamResource() { Name = "Test" }).Result;
        }

        [Test]
        [TestCase("Spaces-2", false, TestName = "Spacific spaces")]
        [TestCase(null, true, TestName = "Specific space with system")]
        [TestCase("Spaces-2", true, TestName = "Specific spaces with system")]
        public void SpaceIdInResourceOutsideOfTheSpaceContextShouldThrowMismatchSpaceContextException(string includingSpaceId, bool includeSystem)
        {
            var client = SetupAsyncClient("Spaces-1");
            client.Create(Arg.Any<string>(), Arg.Any<TeamResource>());
            var teamRepo = new TeamsRepository(OctopusAsyncRepository.Create(client).Result);
            var multiScoped = teamRepo.Including(new SpaceContext(new[] {includingSpaceId}, includeSystem));
            Func<Task<TeamResource>> exec = () => multiScoped.Create(new TeamResource() { Name = "Test", SpaceId = "Spaces-NotWithinContext" });
            exec.ShouldThrow<MismatchSpaceContextException>();
        }
    }
}
