using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Model
{
    public class InterruptionResourceFixture
    {
        [Test]
        public void InterruptionTypeCanBeDeserialized()
        {
            var payload = @"{ ""Title"": ""The deployment requires failure guidance"", ""Type"": ""foo""}";

            var result = JsonSerialization.DeserializeObject<InterruptionResource>(payload);
            result.Type.Should().Be(new InterruptionType("Foo"));
        }
        
        [TestCase("Stop, drop and roll", "ManualIntervention")]
        [TestCase("The deployment requires failure guidance", "GuidedFailure")]
        public void InterruptionTypeIsDerivedFromTheTitleWhenItIsNotSupplied(string title, string expectedType)
        {
            var payload = $@"{{ ""Title"": ""{title}""}}";

            var result = JsonSerialization.DeserializeObject<InterruptionResource>(payload);
            result.Type.Should().Be(new InterruptionType(expectedType));
        }

        [Test]
        public void InterruptionTypeIsDefaultedOnConstruction()
        {
            new InterruptionResource()
                .Type
                .Should()
                .Be(InterruptionType.ManualIntervention);
        }
        
    }
}