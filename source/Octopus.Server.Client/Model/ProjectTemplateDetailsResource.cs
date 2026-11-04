using System;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class ProjectTemplateDetailsResource
    {
        [JsonConstructor]
        public ProjectTemplateDetailsResource(string slug, string versionMask)
        {
            Slug = slug;
            VersionMask = versionMask;
        }

        [JsonProperty(Order = 1)]
        public string Slug { get; set; }

        [JsonProperty(Order = 2)]
        public string VersionMask { get; set; }

        [JsonProperty(Order = 3)]
        public bool IsShared { get; set; }

        /// <summary>
        /// True when the resolved template version manages channels and lifecycles. Output only; ignored on input.
        /// </summary>
        [JsonProperty(Order = 4)]
        public bool ManagedChannelsAndLifecycles { get; set; }
    }
}
