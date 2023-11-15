using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Extensibility;
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
            repository.LoadRootDocument().Returns(GetRootResource());
            ITeamsRepository teamRepo = new TeamsRepository(repository);
            Action switchContext = () => teamRepo.UsingContext(SpaceContext.AllSpaces());
            switchContext.Should().Throw<SpaceContextSwitchException>();
        }

        [Test]
        public void CanSwitchSpaceContextWhenRepositoryScopeIsUnspecified()
        {
            var repository = Substitute.For<IOctopusAsyncRepository>();
            repository.Scope.Returns(RepositoryScope.Unspecified());
            repository.LoadRootDocument().Returns(GetRootResource());
            repository.Teams.UsingContext(SpaceContext.AllSpaces());
        }

        static IEnumerable<TestCaseData> TestCases()
        {
            return new[]
            {
                new TestCaseData(RepositoryScope.ForSystem()),
                new TestCaseData(RepositoryScope.ForSpace(new SpaceResource {Id = "Spaces-1", Links = new LinkCollection {{ "SpaceHome", String.Empty}}}))
            };
        }

        static RootResource GetRootResource()
        {
            return new RootResource
            {
                ApiVersion = "3.0.0",
                Version = "2099.0.0"
            };
        }
    }
}
