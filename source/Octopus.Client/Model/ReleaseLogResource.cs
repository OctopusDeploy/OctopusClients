using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class ReleaseLogResource : Resource, IHaveSpaceResource
    {
        internal static string RequiresOctopusVersion = "2019.11.0";
        internal static string RequiresOctopusVersionMessage = $"Fetching release logs requires Octopus version {RequiresOctopusVersion} or newer";

        public string SpaceId { get; set; }
        public string ReleaseId { get; set; }
        public Guid RequestId { get; set; }
        public DateTimeOffset Occurred { get; set; }
        public List<ReleaseLogEntry> Entries { get; set; } = new List<ReleaseLogEntry>();
    }

    public class ReleaseLogEntry
    {
        public DateTimeOffset Occurred { get; set; }
        public LogCategory Category { get; set; }
        public string Source { get; set; }
        public string MessageText { get; set; }
    }

    /// <summary>
    /// Equivalent to Octopus.Diagnostics.LogCategory. Duplicated here to minimize dependencies.
    /// </summary>
    public enum LogCategory
    {
        Trace = 1,
        Verbose = 100,
        Info = 200,
        Planned = 201,
        Highlight = 210,
        Abandoned = 220,
        Wait = 225,
        Progress = 230,
        Finished = 240,
        Warning = 300,
        Error = 400,
        Fatal = 500
    }
}