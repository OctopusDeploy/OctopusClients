using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public class ContinuousDailyScheduledTriggerFilterResource : ScheduledTriggerFilterResource
    {
        public override TriggerFilterType FilterType => TriggerFilterType.ContinuousDailySchedule;

        [Writeable]
        public DateTime? RunAfter { get; set; }

        [Writeable]
        public DateTime? RunUntil { get; set; }

        [Writeable]
        public DailyScheduledTriggerInterval Interval { get; set; }

        [Writeable]
        public short? HourInterval { get; set; }

        [Writeable]
        public short? MinuteInterval { get; set; }

        [Writeable]
        public bool Monday { get; set; }

        [Writeable]
        public bool Tuesday { get; set; }

        [Writeable]
        public bool Wednesday { get; set; }

        [Writeable]
        public bool Thursday { get; set; }

        [Writeable]
        public bool Friday { get; set; }

        [Writeable]
        public bool Saturday { get; set; }

        [Writeable]
        public bool Sunday { get; set; }

        public void SetDayFromString(string day)
        {
            switch (day.ToLowerInvariant())
            {
                case "sun":
                case "0":
                    Sunday = true;
                    break;
                case "mon":
                case "1":
                    Monday = true;
                    break;
                case "tue":
                case "2":
                    Tuesday = true;
                    break;
                case "wed":
                case "3":
                    Wednesday = true;
                    break;
                case "thu":
                case "4":
                    Thursday = true;
                    break;
                case "fri":
                case "5":
                    Friday = true;
                    break;
                case "sat":
                case "6":
                    Saturday = true;
                    break;
            }
        }
    }
}
