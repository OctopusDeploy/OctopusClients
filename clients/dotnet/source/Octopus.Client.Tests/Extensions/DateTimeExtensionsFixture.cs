using System;
using FluentAssertions;
using NodaTime;
using NUnit.Framework;
using Octopus.Client.Extensions;

namespace Octopus.Client.Tests.Extensions
{
    public class DateTimeExtensionsFixture
    {
        [Test]
        [TestCase("UTC", "UTC")]
        [TestCase("UTC-09", "UTC-09")]
        [TestCase("Australia/Brisbane", "Australia/Brisbane")]
        [TestCase("E. Australia Standard Time", "Australia/Brisbane")]
        [TestCase("Pacific Standard Time (Mexico)", "America/Tijuana")]
        [TestCase("Africa/Bissau", "Africa/Bissau")]
        [TestCase("Etc/GMT+8", "Etc/GMT+8")]
        [TestCase("Invalid", null)]
        public void ToDateTimeZoneOrNull(string tzId, string expected)
        {
            var expectedTz = expected == null
                ? null
                : DateTimeZoneProviders.Tzdb.GetZoneOrNull(expected) ??
                  throw new Exception("Unknown time zone supplied for the expected value");

            tzId
                .ToDateTimeZoneOrNull()
                .Should()
                .Be(expectedTz);
        }

        [Test]
        public void TzdbZoneToWindowsTimeZoneId()
        {
            var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Australia/Brisbane") ??
                       throw new Exception("Unknown time zone supplied for the expected value");

            zone.ToWindowsTimeZoneId()
                .Should()
                .Be("E. Australia Standard Time");
        }
    }
}