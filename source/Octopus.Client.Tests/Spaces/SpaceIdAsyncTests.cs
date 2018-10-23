using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
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
        public async Task MixedScoped_SingleSpaceContextShouldEnrichSpaceId(string spaceId)
        {
            var client = SetupAsyncClient(spaceId);
            await client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t =>
            {
                t.SpaceId.Should().Be(spaceId);
            }));
            var teamRepo = new TeamsRepository(new OctopusAsyncRepository(client, SpaceContext.SpecificSpace(spaceId)));
            var created = await teamRepo.Create(new TeamResource() { Name = "Test" });
        }

        [Test]
        [TestCase("Spaces-1", TestName = "Space1")]
        [TestCase("Spaces-2", TestName = "Space2")]
        public async Task SpaceScoped_SingleSpaceContextShouldNotEnrichSpaceId(string spaceId)
        {
            var client = SetupAsyncClient(spaceId);
            await client.Create(Arg.Any<string>(), Arg.Do<ProjectGroupResource>(t =>
            {
                t.SpaceId.Should().BeNullOrEmpty();
            }));
            var repo = new ProjectGroupRepository(new OctopusAsyncRepository(client, SpaceContext.SpecificSpace(spaceId)));
            var _ = await repo.Create(new ProjectGroupResource { Name = "Test" });
        }

        [Test]
        [TestCase("Spaces-2", false, TestName = "Spacific spaces")]
        [TestCase("Spaces-2", true, TestName = "Specific space with system")]
        [TestCase("Spaces-2", true, TestName = "Specific spaces with system")]
        [TestCase(null, true, TestName = "System only")]
        public async Task NonSingleSpaceContextShouldNotEnrichSpaceId(string includingSpaceId, bool includeSystem)
        {
            var client = SetupAsyncClient("Spaces-1");
            await client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t =>
            {
                t.SpaceId.Should().BeNullOrEmpty();
            }));
            var includingSpaceContext = new SpaceContext(new []{includingSpaceId}, includeSystem);
            var teamRepo = new TeamsRepository(new OctopusAsyncRepository(client, SpaceContext.SystemOnly()));
            var multiScoped = teamRepo.Including(includingSpaceContext);
            var _ = await multiScoped.Create(new TeamResource() { Name = "Test" });
        }

        [Test]
        [TestCase("Spaces-2", false, TestName = "Spacific spaces")]
        [TestCase("Spaces-1", true, TestName = "Specific space with system")]
        [TestCase("Spaces-2", true, TestName = "Specific spaces with system")]
        public async Task NonSingleSpaceContextShouldNotChangeExistingSpaceId(string includingSpaceId, bool includeSystem)
        {
            var client = SetupAsyncClient("Spaces-1");
            await client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t => t.SpaceId.Should().BeNullOrEmpty()));
            var teamRepo = new TeamsRepository(new OctopusAsyncRepository(client, SpaceContext.SystemOnly()));
            var multiScoped = teamRepo.Including(new SpaceContext(new[] {includingSpaceId}, includeSystem));
            var _ = multiScoped.Create(new TeamResource() { Name = "Test" }).Result;
        }
    }
}
