using System;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class ActivityLogElement
    {
        public string Category { get; set; }
        public DateTimeOffset? OccurredAt { get; set; }
        public string MessageText { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Detail { get; set; }
    }
}