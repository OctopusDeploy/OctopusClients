using System;
using FluentAssertions;
using NUnit.Framework;

namespace Octopus.Client.Tests.Spaces
{
    [TestFixture]
    public class SpaceContextTests
    {
        [Test]
        public void ShouldConcatSpaceId()
        {
            var singleSpaceContext1 = new SpaceContext(new []{"Spaces-1"}, false);
            var singleSpaceContext2 = new SpaceContext(new[] { "Spaces-2" }, false);
            singleSpaceContext1.Union(singleSpaceContext2).SpaceIds.Should()
                .BeEquivalentTo(new[] {"Spaces-1", "Spaces-2"});
            singleSpaceContext2.Union(singleSpaceContext1).SpaceIds.Should()
                .BeEquivalentTo(new[] {"Spaces-2", "Spaces-1"});
        }

        [Test]
        public void ShouldRemainIncludeSystemAfterUnionIfOneOfThemIsSetToInclude()
        {
            var singleSpaceContext1 = new SpaceContext(new [] {"Spaces-1"}, false);
            var systemContext = new SpaceContext(new string[0], true);
            singleSpaceContext1.Union(systemContext).IncludeSystem.Should().Be(true);
        }

        [Test]
        public void ShouldRemainIncludeSystemAfterUnionIfBothOfThemIsSetToInclude()
        {
            var singleSpaceContext1 = new SpaceContext(new[] { "Spaces-1" }, true);
            var systemContext = new SpaceContext(new string[0], true);
            singleSpaceContext1.Union(systemContext).IncludeSystem.Should().Be(true);
        }

        [Test]
        public void ShouldNotIncludeSystemAfterUnionIfBothOfThemIsSetToNotInclude()
        {
            var singleSpaceContext1 = new SpaceContext(new[] { "Spaces-1" }, false);
            var systemContext = new SpaceContext(new [] {"Spaces-2"}, false);
            singleSpaceContext1.Union(systemContext).IncludeSystem.Should().Be(false);
        }

        [Test]
        public void NoSpaceAndNotIncludeSystemShouldThrow()
        {
           Action createContext = () => new SpaceContext(new string[0], false);
            createContext.ShouldThrow<ArgumentException>(
                "At least 1 spaceId is required when includeSystem is set to false");
        }
    }
}
