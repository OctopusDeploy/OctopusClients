﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Tests.Spaces
{
    [TestFixture]
    public class SpaceIdAsyncTests
    {
        IOctopusAsyncClient SetupAsyncClient()
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            client.Get<UserResource>(Arg.Any<string>()).Returns(new UserResource() { Links = { { "Spaces", "" } } });
            client.Get<SpaceResource[]>(Arg.Any<string>()).Returns(new[] { new SpaceResource { Id = "Spaces-1", IsDefault = true, Links = new LinkCollection{{"SpaceHome", String.Empty}}} });
            client.Get<SpaceRootResource>(Arg.Any<string>(), Arg.Any<object>()).Returns(new SpaceRootResource());
            client.Get<RootResource>(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(new RootResource()
            {
                ApiVersion = "3.0.0",
                Version = "2099.0.0",
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
            var client = SetupAsyncClient();
            await client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t =>
            {
                t.SpaceId.Should().Be(spaceId);
            })).ConfigureAwait(false);
            var teamRepo = new TeamsRepository(new OctopusAsyncRepository(client, RepositoryScope.ForSpace(CreateSpaceResource(spaceId))));
            var created = await teamRepo.Create(new TeamResource() { Name = "Test" }).ConfigureAwait(false);
        }

        [Test]
        [TestCase("Spaces-1", TestName = "Space1")]
        [TestCase("Spaces-2", TestName = "Space2")]
        public async Task SpaceScoped_SingleSpaceContextShouldEnrichSpaceId(string spaceId)
        {
            var client = SetupAsyncClient();
            await client.Create(Arg.Any<string>(), Arg.Do<ProjectGroupResource>(t =>
            {
                t.SpaceId.Should().Be(spaceId);
            }));
            var repo = new ProjectGroupRepository(new OctopusAsyncRepository(client, RepositoryScope.ForSpace(CreateSpaceResource(spaceId))));
            var _ = await repo.Create(new ProjectGroupResource { Name = "Test" }).ConfigureAwait(false);
        }

        [Test]
        [TestCase("Spaces-2", true, TestName = "Specific space with system")]
        [TestCase("Spaces-2", true, TestName = "Specific spaces with system")]
        [TestCase(null, true, TestName = "System only")]
        public async Task NonSingleSpaceContextShouldNotEnrichSpaceId(string includingSpaceId, bool includeSystem)
        {
            var client = SetupAsyncClient();
            await client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t =>
            {
                t.SpaceId.Should().BeNullOrEmpty();
            })).ConfigureAwait(false);
            var includingSpaceContext = includeSystem ? SpaceContext.SpecificSpaceAndSystem(CreateSpaceResource(includingSpaceId)) : SpaceContext.SpecificSpace(CreateSpaceResource(includingSpaceId));
            var teamRepo = new TeamsRepository(new OctopusAsyncRepository(client, RepositoryScope.Unspecified()));
            var multiScoped = teamRepo.UsingContext(includingSpaceContext);
            var _ = await multiScoped.Create(new TeamResource() { Name = "Test" }).ConfigureAwait(false);
        }

        [Test]
        [TestCase("Spaces-2", true, TestName = "Spacific spaces")]
        [TestCase("Spaces-1,Spaces-2", true, TestName = "Specific spaces")]
        [TestCase("Spaces-2", true, TestName = "Specific spaces with system")]
        public async Task NonSingleSpaceContextShouldNotChangeExistingSpaceId(string spaceIdsToUse, bool includeSystem)
        {
            var spaceIds = spaceIdsToUse.Split(',');
            var client = SetupAsyncClient();
            await client.Create(Arg.Any<string>(), Arg.Do<TeamResource>(t => t.SpaceId.Should().Be("Spaces-4"))).ConfigureAwait(false);
            var teamRepo = new TeamsRepository(new OctopusAsyncRepository(client, RepositoryScope.Unspecified()));
            var multiScoped = teamRepo.UsingContext(SpaceContext.SpecificSpacesAndSystem(spaceIds.Select(CreateSpaceResource)));
            var _ = await multiScoped.Create(new TeamResource() { Name = "Test", SpaceId = "Spaces-4" }).ConfigureAwait(false);
        }

        SpaceResource CreateSpaceResource(string id)
        {
            return new SpaceResource
            {
                Id = id,
                Links = new LinkCollection{{"SpaceHome", String.Empty}}
            };
        }
    }
}
