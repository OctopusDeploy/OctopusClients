using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class BackupConfigurationResource : Resource
    {
        [Writeable]
        public string BackupToDirectory { get; set; }

        [Writeable]
        public bool IsMasterKeyBackedUp { get; set; }

        [Writeable]
        public bool BackupAutomatically { get; set; }

        [Writeable]
        public TimeSpan? BackupEvery { get; set; }

        [Writeable]
        public TimeSpan? StartingFrom { get; set; }

        [Writeable]
        public RetentionPeriod Retention { get; set; }

        public DateTimeOffset? NextDue { get; set; }
    }
}