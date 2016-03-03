using System;

namespace Octopus.Cli.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string Friendly(this TimeSpan time)
        {
            if (time.TotalDays >= 1.0)
                return string.Format("{0:n0} day{1}, {2:n0}h:{3:n0}m:{4:n0}s", time.TotalDays, time.TotalDays >= 1.9 ? "s" : "", time.Hours, time.Minutes, time.Seconds);

            if (time.TotalHours >= 1.0)
                return string.Format("{0:n0}h:{1:n0}m:{2:n0}s", time.TotalHours, time.Minutes, time.Seconds);

            if (time.TotalMinutes >= 1.0)
                return string.Format("{0:n0}m:{1:n0}s", time.TotalMinutes, time.Seconds);

            return string.Format("{0:n0}s", time.TotalSeconds);
        }
    }
}