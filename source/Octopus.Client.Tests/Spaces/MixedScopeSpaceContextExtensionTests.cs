using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Tests.Spaces
{
    [TestFixture]
    public class MixedScopeSpaceContextExtensionTests
    {
        [Test]
        public void CanExtendSpaceContextAsync()
        {
            var repository = Substitute.For<IOctopusAsyncRepository>();
            repository.SpaceContext.Returns(SpaceContext.SpecificSpaceAndSystem("Spaces-1"));
            ITeamsRepository teamRepo = new TeamsRepository(repository);
            teamRepo = new[] {"Spaces-2", "Spaces-3"}.Aggregate(teamRepo,
                (repo, spaceId) => repo.Including(SpaceContext.SpecificSpace(spaceId)));
            var additionalQueryParameters =
                teamRepo.GetType().GetTypeInfo().BaseType.GetProperty("AdditionalQueryParameters", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(teamRepo) as Dictionary<string, object>;
            additionalQueryParameters["includeSystem"].Should().Be(true);
            additionalQueryParameters["spaces"].ShouldBeEquivalentTo(new[] { "Spaces-1", "Spaces-2", "Spaces-3" });
        }
    }
}
