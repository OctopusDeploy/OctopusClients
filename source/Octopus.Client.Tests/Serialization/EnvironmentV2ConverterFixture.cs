using System;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Serialization
{
    [TestFixture]
    public class EnvironmentV2ConverterFixture
    {
        [Test]
        public void StaticEnvironmentCanBeConvertedSuccessfully()
        {
            var input = new
            {
                Id = "Environments-1",
                Type = nameof(EnvironmentType.Static),
                SpaceId = "Spaces-1",
                Slug = "production",
                Name = "Production",
                Description = "The production environment",
                EnvironmentTags = new[] { "prod" },
                SortOrder = 3,
                UseGuidedFailure = true,
                AllowDynamicInfrastructure = true
            };

            var result = Execute<StaticEnvironmentV2Resource>(input);

            result.Id.Should().Be(input.Id);
            result.SpaceId.Should().Be(input.SpaceId);
            result.Slug.Should().Be(input.Slug);
            result.Name.Should().Be(input.Name);
            result.Description.Should().Be(input.Description);
            result.EnvironmentTags.Should().BeEquivalentTo(input.EnvironmentTags);
            result.SortOrder.Should().Be(input.SortOrder);
            result.UseGuidedFailure.Should().Be(input.UseGuidedFailure);
            result.AllowDynamicInfrastructure.Should().Be(input.AllowDynamicInfrastructure);
        }

        [Test]
        public void ParentEnvironmentCanBeConvertedSuccessfully()
        {
            var input = new
            {
                Id = "Environments-2",
                Type = nameof(EnvironmentType.Parent),
                SpaceId = "Spaces-1",
                Slug = "staging-parent",
                Name = "Staging",
                SortOrder = 5,
                UseGuidedFailure = false
            };

            var result = Execute<ParentEnvironmentV2Resource>(input);

            result.Id.Should().Be(input.Id);
            result.Name.Should().Be(input.Name);
            result.SortOrder.Should().Be(input.SortOrder);
            result.UseGuidedFailure.Should().Be(input.UseGuidedFailure);
        }

        [Test]
        public void EphemeralEnvironmentCanBeConvertedSuccessfully()
        {
            var input = new
            {
                Id = "Environments-3",
                Type = nameof(EnvironmentType.Ephemeral),
                SpaceId = "Spaces-1",
                Slug = "pr-123",
                Name = "PR-123",
                SortOrder = int.MaxValue,
                ParentEnvironmentId = "Environments-2"
            };

            var result = Execute<EphemeralEnvironmentV2Resource>(input);

            result.Id.Should().Be(input.Id);
            result.Name.Should().Be(input.Name);
            result.ParentEnvironmentId.Should().Be(input.ParentEnvironmentId);
        }

        [Test]
        public void UnrecognisedEnvironmentTypeThrowsOnConversion()
        {
            var input = new
            {
                Id = "Environments-4",
                Type = "SomeFutureType",
                SpaceId = "Spaces-1",
                Slug = "future",
                Name = "Future"
            };

            var json = JsonConvert.SerializeObject(input);
            var settings = JsonSerialization.GetDefaultSerializerSettings();

            Action act = () => JsonConvert.DeserializeObject<BaseEnvironmentV2Resource>(json, settings);

            act.Should().Throw<Exception>();
        }

        private static T Execute<T>(object input)
        {
            var json = JsonConvert.SerializeObject(input);

            var settings = JsonSerialization.GetDefaultSerializerSettings();
            return JsonConvert.DeserializeObject<BaseEnvironmentV2Resource>(json, settings)
                .Should().BeOfType<T>().Subject;
        }
    }
}
