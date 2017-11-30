﻿namespace Octopus.Client.Model.ScheduledTriggers
{
    public class DaysPerWeekTriggerScheduleResource : DailyTriggerScheduleResource
    {
        public override TriggerScheduleType ScheduleType => TriggerScheduleType.DaysPerWeek;

        public bool Monday { get; set; }

        public bool Tuesday { get; set; }

        public bool Wednesday { get; set; }

        public bool Thursday { get; set; }

        public bool Friday { get; set; }

        public bool Saturday { get; set; }

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
