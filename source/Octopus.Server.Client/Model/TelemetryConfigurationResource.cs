using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class TelemetryConfigurationResource: Resource
    {
        [Writeable]
        public bool Enabled { get; set; }

        public DateTimeOffset ShowAsNewUntil { get; set; }
    }
}