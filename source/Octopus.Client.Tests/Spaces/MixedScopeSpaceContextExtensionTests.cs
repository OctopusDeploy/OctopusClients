using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Tests.Spaces
{
    [TestFixture]
    public class MixedScopeSpaceContextExtensionTests
    {
        [Test, TestCaseSource(nameof(TestCases))]
        public void CannotSwitchSpaceContextWhenTheRepositoryScopeIsSpecified(RepositoryScope scope)
        {
            var repository = Substitute.For<IOctopusAsyncRepository>();
            repository.Scope.Returns(scope);
            ITeamsRepository teamRepo = new TeamsRepository(repository);
            Action switchContext = () => teamRepo.UsingContext(SpaceContext.AllSpaces());
            switchContext.ShouldThrow<SpaceContextSwitchException>();
        }

        [Test]
        public void CanSwitchSpaceContextWhenRepositoryScopeIsUnspecified()
        {
            var repository = Substitute.For<IOctopusAsyncRepository>();
            repository.Scope.Returns(RepositoryScope.Unspecified());
            repository.Teams.UsingContext(SpaceContext.AllSpaces());
        }

        static IEnumerable<TestCaseData> TestCases()
        {
            return new[]
            {
                new TestCaseData(RepositoryScope.ForSystem()),
                new TestCaseData(RepositoryScope.ForSpace(new SpaceResource().WithId("Spaces-1")))
            };
        }
    }
}
