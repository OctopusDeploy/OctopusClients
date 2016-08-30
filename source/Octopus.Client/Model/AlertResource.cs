using System;

namespace Octopus.Client.Model
{
    public class AlertResource : Resource
    {
        public AlertSeverity Severity { get; set; }
        public string Message { get; set; }
    }
}