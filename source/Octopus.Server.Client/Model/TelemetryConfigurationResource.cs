using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class TelemetryConfigurationResource : Resource
    {
        [Writeable]
        public bool Enabled { get; set; }

        /// <summary>
        /// Which telemetry is sent when enabled: "All" or "UsageOnly". Leave null when updating to keep the existing mode.
        /// </summary>
        [Writeable]
        public string Mode { get; set; }

        public DateTimeOffset ShowAsNewUntil { get; set; }

        public bool IsTelemetryEnforced { get; set; }
    }
}
