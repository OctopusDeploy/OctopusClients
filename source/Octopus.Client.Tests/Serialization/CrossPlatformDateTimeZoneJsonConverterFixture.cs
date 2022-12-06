using System;
using FluentAssertions;
using Newtonsoft.Json;
using NodaTime;
using NUnit.Framework;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Serialization
{
    [TestFixture]
    public class CrossPlatformDateTimeZoneJsonConverterFixture
    {
        readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            Converters =
            {
                new CrossPlatformDateTimeZoneJsonConverter()
            },
        };

        [Test]
        [TestCase("2022-08-24T20:54:46.477+8:00", true)]
        [TestCase("2022-08-24T12:54:46Z", true)]
        [TestCase("2022-08-24T20:54:46", false)]
        public void ShouldDeserializeDateTimeOffset(string serializedDate, bool hasTimeZone)
        {
            var date = JsonConvert.DeserializeObject<DateTimeOffset>($@"""{serializedDate}""", serializerSettings);

            var expectedDateWithoutOffset = new DateTime(
                2022,
                08,
                24,
                20,
                54,
                46);
            var expected = hasTimeZone 
                ? new DateTimeOffset(expectedDateWithoutOffset, TimeSpan.FromHours(8)) 
                : new DateTimeOffset(expectedDateWithoutOffset);

            date
                .Should()
                .BeCloseTo(expected, 1000); // Within one second
        }

        [Test]
        public void ShouldDeserializeDateTime()
        {
            var date = JsonConvert.DeserializeObject<DateTime>(@"""2022-08-24T12:54:46Z""", serializerSettings);

            var expected = new DateTime(
                2022,
                08,
                24,
                12,
                54,
                46);

            date
                .Should()
                .BeCloseTo(expected, 1000); // Within one second
        }

        [Test]
        public void ShouldDeserializeDateTimeWithKindOfUtc()
        {
            var date = JsonConvert.DeserializeObject<DateTime>(@"""2022-08-24T12:54:46Z""", serializerSettings);
            date
                .Kind
                .Should()
                .Be(DateTimeKind.Utc);
        }

        [Test]
        [TestCase("Australia/Brisbane")] // IANA Zone
        [TestCase("E. Australia Standard Time")] // Windows (BCL) zone
        public void ShouldDeserializeDateTimeZone(string zone)
        {
            var result = JsonConvert.DeserializeObject<DateTimeZone>($@"""{zone}""", serializerSettings);
            result.Should()
                .Be(DateTimeZoneProviders.Tzdb.GetZoneOrNull("Australia/Brisbane"));
        }

        [Test]
        public void ShouldDeserializeLocalDate()
        {
            var result = JsonConvert.DeserializeObject<LocalDate>(@"""2022-04-03""", serializerSettings);
            result.Should()
                .Be(new LocalDate(2022, 4, 3));
        }
    }
}