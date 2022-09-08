using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class UpgradeConfigurationResource : Resource
    {
        [Writeable]
        public UpgradeNotificationMode NotificationMode { get; set; }

        [Writeable]
        public bool AllowChecking { get; set; }

        [Writeable]
        [Obsolete("Replaced by " + nameof(TelemetryConfigurationResource) + "." + nameof(TelemetryConfigurationResource.Enabled))]
        public bool IncludeStatistics { get; set; }
    }
}