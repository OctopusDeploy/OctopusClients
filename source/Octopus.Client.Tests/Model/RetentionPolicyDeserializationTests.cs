using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Model.SpaceDefaultRetentionPolicies;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Model
{
    public class RetentionPolicyDeserializationTests
    {
        [Test]
        public void LifecycleRetentionPolicyTypeCanBeDeserialized()
        {
            var payload = @"{ ""Id"": ""LifecycleReleaseRetentionDefault-Spaces-9"", ""Name"": ""Space Default Release Lifecycle Retention Policy"", ""RetentionType"": ""LifecycleRelease"", ""SpaceId"": ""Spaces-9"", ""Strategy"": ""Count"" }";

            var result = JsonSerialization.DeserializeObject<SpaceDefaultRetentionPolicyResource>(payload);
            result.RetentionType.Should().Be(RetentionType.LifecycleRelease);
            result.SpaceId.Should().Be("Spaces-9");
            result.GetType().Should().Be(typeof(SpaceDefaultLifecycleReleaseRetentionPolicyResource));
            
            var lifecycleResult = result as SpaceDefaultLifecycleReleaseRetentionPolicyResource;
            lifecycleResult?.Strategy.Should().Be(RetentionPeriodStrategy.Count);
            lifecycleResult?.QuantityToKeep.HasValue.Should().BeFalse();
            lifecycleResult?.Unit?.Value.Should().BeNull();
        }

        [Test]
        public void TentacleRetentionPolicyTypeCanBeDeserialized()
        {
            var payload = @"{ ""Id"": ""LifecycleTentacleRetentionDefault-Spaces-9"", ""Name"": ""Space Default Lifecycle Tentacle Retention Policy"", ""RetentionType"": ""LifecycleTentacle"", ""SpaceId"": ""Spaces-9"", ""Strategy"":""Forever"" }";

            var result = JsonSerialization.DeserializeObject<SpaceDefaultRetentionPolicyResource>(payload);
            result.RetentionType.Should().Be(RetentionType.LifecycleTentacle);
            result.SpaceId.Should().Be("Spaces-9");
            result.GetType().Should().Be(typeof(SpaceDefaultLifecycleTentacleRetentionPolicyResource));
            
            var tentacleResult = result as SpaceDefaultLifecycleTentacleRetentionPolicyResource;
            tentacleResult?.Strategy.Should().Be(RetentionPeriodStrategy.Forever);
        }
    }
}
