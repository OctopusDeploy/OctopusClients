using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Model.DeploymentFreezes;
using Octopus.Client.Model.Triggers.ScheduledTriggers;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Serialization
{
    [TestFixture]
    public class RecurringScheduleConverterFixture
    {
        [Test]
        public void DailyRecurringSchedule_Deserializes()
        {
            var input = new
            {
                Type = "Daily",
                Unit = 2,
                EndType = "Never",
                UserUtcOffsetInMinutes = 0
            };

            var result = Execute<DailyRecurringSchedule>(input);

            result.Type.Should().Be(RecurringScheduleType.Daily);
            result.Unit.Should().Be(2);
            result.EndType.Should().Be(RecurringScheduleEndType.Never);
        }

        [Test]
        public void WeeklyRecurringSchedule_Deserializes()
        {
            var input = new
            {
                Type = "Weekly",
                Unit = 1,
                DaysOfWeek = new[] { "Monday", "Wednesday", "Friday" },
                EndType = "AfterOccurrences",
                EndAfterOccurrences = 5,
                UserUtcOffsetInMinutes = -480
            };

            var result = Execute<WeeklyRecurringSchedule>(input);

            result.Type.Should().Be(RecurringScheduleType.Weekly);
            result.Unit.Should().Be(1);
            result.DaysOfWeek.Should().HaveFlag(DaysOfWeek.Monday);
            result.DaysOfWeek.Should().HaveFlag(DaysOfWeek.Wednesday);
            result.DaysOfWeek.Should().HaveFlag(DaysOfWeek.Friday);
            result.EndType.Should().Be(RecurringScheduleEndType.AfterOccurrences);
            result.EndAfterOccurrences.Should().Be(5);
        }

        [Test]
        public void MonthlyRecurringSchedule_Deserializes()
        {
            var input = new
            {
                Type = "Monthly",
                Unit = 2,
                MonthlyScheduleType = "DayOfMonth",
                DayNumberOfMonth = "First",
                DayOfWeek = 1, // Monday
                EndType = "Never",
                UserUtcOffsetInMinutes = 0
            };

            var result = Execute<MonthlyRecurringSchedule>(input);

            result.Type.Should().Be(RecurringScheduleType.Monthly);
            result.Unit.Should().Be(2);
            result.MonthlyScheduleType.Should().Be(MonthlyScheduleType.DayOfMonth);
            result.DayNumberOfMonth.Should().Be("First");
            result.EndType.Should().Be(RecurringScheduleEndType.Never);
        }

        [Test]
        public void AnnuallyRecurringSchedule_Deserializes()
        {
            var input = new
            {
                Type = "Annually",
                Unit = 1,
                EndType = "OnDate",
                EndOnDate = "2026-12-31T23:59:59Z",
                UserUtcOffsetInMinutes = 0
            };

            var result = Execute<AnnuallyRecurringSchedule>(input);

            result.Type.Should().Be(RecurringScheduleType.Annually);
            result.Unit.Should().Be(1);
            result.EndType.Should().Be(RecurringScheduleEndType.OnDate);
            result.EndOnDate.Should().NotBeNull();
        }

        [Test]
        public void DailyRecurringSchedule_Serializes()
        {
            var schedule = new DailyRecurringSchedule
            {
                Unit = 3,
                EndType = RecurringScheduleEndType.AfterOccurrences,
                EndAfterOccurrences = 10,
                UserUtcOffsetInMinutes = 60
            };

            var json = JsonConvert.SerializeObject(schedule, JsonSerialization.GetDefaultSerializerSettings());
            var result = JsonConvert.DeserializeObject<RecurringSchedule>(json, JsonSerialization.GetDefaultSerializerSettings());

            result.Should().BeOfType<DailyRecurringSchedule>();
            result.Unit.Should().Be(3);
            result.EndAfterOccurrences.Should().Be(10);
        }

        [Test]
        public void WeeklyRecurringSchedule_Serializes()
        {
            var schedule = new WeeklyRecurringSchedule
            {
                Unit = 2,
                DaysOfWeek = DaysOfWeek.Monday | DaysOfWeek.Friday,
                EndType = RecurringScheduleEndType.Never,
                UserUtcOffsetInMinutes = 0
            };

            var json = JsonConvert.SerializeObject(schedule, JsonSerialization.GetDefaultSerializerSettings());
            var result = JsonConvert.DeserializeObject<RecurringSchedule>(json, JsonSerialization.GetDefaultSerializerSettings());

            result.Should().BeOfType<WeeklyRecurringSchedule>();
            var weekly = (WeeklyRecurringSchedule)result;
            weekly.DaysOfWeek.Should().HaveFlag(DaysOfWeek.Monday);
            weekly.DaysOfWeek.Should().HaveFlag(DaysOfWeek.Friday);
        }

        private static T Execute<T>(object input) where T : RecurringSchedule
        {
            // Serialize anonymous object to JSON
            var json = JsonConvert.SerializeObject(input);

            var settings = JsonSerialization.GetDefaultSerializerSettings();
            return JsonConvert.DeserializeObject<RecurringSchedule>(json, settings)
                .Should().BeOfType<T>().Subject;
        }
    }
}
