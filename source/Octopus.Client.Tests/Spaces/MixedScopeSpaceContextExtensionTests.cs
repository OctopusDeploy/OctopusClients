using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Exceptions;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Tests.Spaces
{
    [TestFixture]
    public class MixedScopeSpaceContextExtensionTests
    {
        [Test]
        public void CanIncludeMoreSpacesFromSystemOnlyContext()
        {
            var repository = Substitute.For<IOctopusAsyncRepository>();
            repository.Scope.Returns(SpaceContext.SystemOnly());
            ITeamsRepository teamRepo = new TeamsRepository(repository);
            teamRepo = new[] {"Spaces-2", "Spaces-3"}.Aggregate(teamRepo,
                (repo, spaceId) => repo.UsingContext(SpaceContext.SpecificSpace(spaceId)));
            var additionalQueryParameters =
                teamRepo.GetType().GetTypeInfo().BaseType.GetProperty("AdditionalQueryParameters", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(teamRepo) as Dictionary<string, object>;
            additionalQueryParameters["includeSystem"].Should().Be(true);
            additionalQueryParameters["spaces"].ShouldBeEquivalentTo(new[] { "Spaces-2", "Spaces-3" });
        }

        [Test]
        public void CanNotIncludeMoreSpacesFromSpecificSpaceContext()
        {
            var repository = Substitute.For<IOctopusAsyncRepository>();
            repository.Scope.Returns(SpaceContext.SpecificSpace("Spaces-1"));
            ITeamsRepository teamRepo = new TeamsRepository(repository);
            teamRepo = teamRepo.UsingContext(SpaceContext.SpecificSpace("Spaces-2"));
            Action getParameters = () => teamRepo.GetType().GetTypeInfo().BaseType.GetProperty("AdditionalQueryParameters", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(teamRepo);
            getParameters.ShouldThrow<TargetInvocationException>().WithInnerException<SpaceContextExtensionException>();
        }

        [Test]
        public void CanNotIncludeMoreSpacesFromSpecificSpaceAndSystemContext()
        {
            var repository = Substitute.For<IOctopusAsyncRepository>();
            repository.Scope.Returns(SpaceContext.SpecificSpaceAndSystem("Spaces-1"));
            ITeamsRepository teamRepo = new TeamsRepository(repository);
            teamRepo = teamRepo.UsingContext(SpaceContext.SpecificSpace("Spaces-2"));
            Action getParameters = () => teamRepo.GetType().GetTypeInfo().BaseType.GetProperty("AdditionalQueryParameters", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(teamRepo);
            getParameters.ShouldThrow<TargetInvocationException>().WithInnerException<SpaceContextExtensionException>();
        }

        [Test]
        public void CanIncludeSystemFromSpecificSpaceContext()
        {
            var repository = Substitute.For<IOctopusAsyncRepository>();
            repository.Scope.Returns(SpaceContext.SpecificSpace("Spaces-1"));
            ITeamsRepository teamRepo = new TeamsRepository(repository);
            teamRepo = teamRepo.UsingContext(SpaceContext.SystemOnly());

            var additionalQueryParameters =
                teamRepo.GetType().GetTypeInfo().BaseType.GetProperty("AdditionalQueryParameters", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(teamRepo) as Dictionary<string, object>;
            additionalQueryParameters["includeSystem"].Should().Be(true);
            additionalQueryParameters["spaces"].ShouldBeEquivalentTo(new[] { "Spaces-1"});
        }
    }
}
