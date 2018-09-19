using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Tests.Spaces
{
    [TestFixture]
    public class SpaceIdAsyncTests
    {

        IOctopusAsyncClient SetupAsyncClient(string[] spaceIds, bool includeSystem)
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            client.SpaceContext.Returns(new SpaceContext(spaceIds, includeSystem));
            return client;
        }

        [Test]
        [TestCase(new[] { "Spaces-1" }, false, TestName = "Space1")]
        [TestCase(new[] { "Spaces-2" }, false, TestName = "Space2")]
        public void MixedScoped_SingleSpaceContextShouldEnrichSpaceId(string[] spaceIds, bool includeSystem)
        {
            var client = SetupAsyncClient(spaceIds, includeSystem);
            client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t =>
            {
                t.SpaceId.Should().Be(spaceIds.Single());
            }));
            var teamRepo = new TeamsRepository(client);
            var created = teamRepo.Create(new TeamResource() { Name = "Test" }).Result;
        }

        [Test]
        [TestCase(new[] { "Spaces-1" }, false, TestName = "Space1")]
        [TestCase(new[] { "Spaces-2" }, false, TestName = "Space2")]
        public void SpaceScoped_SingleSpaceContextShouldNotEnrichSpaceId(string[] spaceIds, bool includeSystem)
        {
            var client = SetupAsyncClient(spaceIds, includeSystem);
            client.Create(Arg.Any<string>(), Arg.Do<ProjectGroupResource>(t =>
            {
                t.SpaceId.Should().BeNullOrEmpty();
            }));
            var repo = new ProjectGroupRepository(client);
            var _ = repo.Create(new ProjectGroupResource { Name = "Test" }).Result;
        }

        [Test]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, false, TestName = "Spacific spaces")]
        [TestCase(new[] { "Spaces-1" }, true, TestName = "Specific space with system")]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, true, TestName = "Specific spaces with system")]
        [TestCase(new string[0], true, TestName = "System only")]
        public void NonSingleSpaceContextShouldNotEnrichSpaceId(string[] spaceIds, bool includeSystem)
        {
            var client = SetupAsyncClient(spaceIds, includeSystem);
            client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t =>
            {
                t.SpaceId.Should().BeNullOrEmpty();
            }));
            var teamRepo = new TeamsRepository(client);
            var _ = teamRepo.Create(new TeamResource() { Name = "Test" }).Result;
        }

        [Test]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, false, TestName = "Spacific spaces")]
        [TestCase(new[] { "Spaces-1" }, true, TestName = "Specific space with system")]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, true, TestName = "Specific spaces with system")]
        public void NonSingleSpaceContextShouldNotChangeExistingSpaceId(string[] spaceIds, bool includeSystem)
        {
            var client = SetupAsyncClient(spaceIds, includeSystem);
            client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t => t.SpaceId.Should().BeNullOrEmpty()));
            var teamRepo = new TeamsRepository(client);
            var _ = teamRepo.Create(new TeamResource() { Name = "Test" }).Result;
        }

        [Test]
        [TestCase(new[] { "Spaces-1" }, false, TestName = "Specific space")]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, false, TestName = "Spacific spaces")]
        [TestCase(new[] { "Spaces-1" }, true, TestName = "Specific space with system")]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, true, TestName = "Specific spaces with system")]
        [TestCase(new string[0], true, TestName = "System only")]
        public void SpaceIdInResourceOutsideOfTheSpaceContextShouldThrowMismatchSpaceContextException(string[] spaceIds, bool includeSystem)
        {
            var client = SetupAsyncClient(spaceIds, includeSystem);
            client.Create(Arg.Any<string>(), Arg.Any<TeamResource>());
            var teamRepo = new TeamsRepository(client);
            Func<Task<TeamResource>> exec = () => teamRepo.Create(new TeamResource() { Name = "Test", SpaceId = "Spaces-NotWithinContext" });
            exec.ShouldThrow<MismatchSpaceContextException>();
        }





        [Test]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, false, TestName = "Spacific spaces")]
        [TestCase(new[] { "Spaces-1" }, true, TestName = "Specific space with system")]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, true, TestName = "Specific spaces with system")]
        [TestCase(new string[0], true, TestName = "System only")]
        public void SpaceScoped_NonSingleSpaceContextShouldNotEnrichSpaceId(string[] spaceIds, bool includeSystem)
        {
            var client = SetupAsyncClient(spaceIds, includeSystem);
            client.Create(Arg.Any<string>(), Arg.Do<ProjectGroupResource>(t =>
            {
                t.SpaceId.Should().BeNullOrEmpty();
            }));
            var teamRepo = new ProjectGroupRepository(client);
            var _ = teamRepo.Create(new ProjectGroupResource() { Name = "Test" }).Result;
        }

        [Test]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, false, TestName = "Spacific spaces")]
        [TestCase(new[] { "Spaces-1" }, true, TestName = "Specific space with system")]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, true, TestName = "Specific spaces with system")]
        public void SpaceScoped_NonSingleSpaceContextShouldNotChangeExistingSpaceId(string[] spaceIds, bool includeSystem)
        {
            var client = SetupAsyncClient(spaceIds, includeSystem);
            client.Create(Arg.Any<string>(), Arg.Do<ProjectGroupResource>(t => t.SpaceId.Should().BeNullOrEmpty()));
            var teamRepo = new ProjectGroupRepository(client);
            var _ = teamRepo.Create(new ProjectGroupResource() { Name = "Test" }).Result;
        }

        [Test]
        [TestCase(new[] { "Spaces-1" }, false, TestName = "Specific space")]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, false, TestName = "Spacific spaces")]
        [TestCase(new[] { "Spaces-1" }, true, TestName = "Specific space with system")]
        [TestCase(new[] { "Spaces-1", "Spaces-2" }, true, TestName = "Specific spaces with system")]
        [TestCase(new string[0], true, TestName = "System only")]
        public void SpaceScoped_SpaceIdInResourceOutsideOfTheSpaceContextShouldThrowMismatchSpaceContextException(string[] spaceIds, bool includeSystem)
        {
            var client = SetupAsyncClient(spaceIds, includeSystem);
            client.Create(Arg.Any<string>(), Arg.Any<ProjectGroupResource>());
            var teamRepo = new ProjectGroupRepository(client);
            Func<Task<ProjectGroupResource>> exec = () => teamRepo.Create(new ProjectGroupResource() { Name = "Test", SpaceId = "Spaces-NotWithinContext" });
            exec.ShouldThrow<MismatchSpaceContextException>();
        }
    }
}
