using System;
using FluentAssertions;
using Newtonsoft.Json;
using NodaTime;
using NUnit.Framework;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Serialization
{
    public class PermissiveInstantJsonConverterFixture
    {
        readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            Converters =
            {
                new PermissiveInstantJsonConverter(),
            },
        };

        [Test]
        public void ShouldDeserializeInstantWithoutZone()
        {
            var result = JsonConvert.DeserializeObject<Instant?>(@"""2022-08-24T12:54:46""", serializerSettings);
            var expectedDateWithoutOffset = new DateTime(
                2022,
                08,
                24,
                12,
                54,
                46);
            
            var expected = Instant.FromDateTimeUtc(new DateTime(expectedDateWithoutOffset.Ticks, DateTimeKind.Utc))
                .Minus(Duration.FromTimeSpan(new DateTimeOffset(expectedDateWithoutOffset).Offset));

            result
                .Should()
                .Be(expected);
        }

        [Test]
        public void ShouldDeserializeInstantWithUtcZone()
        {
            var result = JsonConvert.DeserializeObject<Instant?>(@"""2022-08-24T12:54:46Z""", serializerSettings);
            var expected = Instant.FromUtc(
                2022,
                8,
                24,
                12,
                54,
                46);

            result
                .Should()
                .Be(expected);
        }

        [Test]
        public void ShouldDeserializeInstantWithTimeZone()
        {
            var result =
                JsonConvert.DeserializeObject<Instant?>(@"""2022-08-24T20:54:46.477+8:00""", serializerSettings);
            var expected = Instant.FromUtc(
                    2022,
                    8,
                    24,
                    12,
                    54,
                    46)
                .Plus(Duration.FromMilliseconds(477));

            result
                .Should()
                .Be(expected);
        }


        [Test]
        public void ShouldDeserializeNullInstant()
        {
            JsonConvert.DeserializeObject<Instant?>(@"null")
                .Should()
                .BeNull();
        }

        [Test]
        public void ShouldFailDeserializingAnInvalidInstant()
        {
            Action action = () => JsonConvert.DeserializeObject<Instant>(@"""Something Else""", serializerSettings);

            action.ShouldThrow<FormatException>()
                .WithMessage("*was not recognized as a valid DateTime*");
        }
    }
}