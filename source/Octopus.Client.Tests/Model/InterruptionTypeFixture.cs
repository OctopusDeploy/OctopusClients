using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Model
{
    public class InterruptionTypeFixture
    {
        [Test]
        public void DifferentValuesAreNotEqual() 
            => new InterruptionType("LoudExplosion")
                .Should()
                .NotBe(new InterruptionType("QuietNudge"));
        
        [Test]
        public void IsNotCaseSensitive() 
            => new InterruptionType("LoudExplosion")
                .Should()
                .Be(new InterruptionType("loudexPlosion"));
    }
}