using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class ProjectTemplateDetailsResource
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected ProjectTemplateDetailsResource()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
        }

        [JsonConstructor]
        public ProjectTemplateDetailsResource(string slug, string versionMask, string currentVersion)
        {
            Slug = slug;
            VersionMask = versionMask;
            CurrentVersion = currentVersion;
        }

        [JsonProperty(Order = 1)]
        [Required]
        public string Slug { get; set; }

        [JsonProperty(Order = 2)]
        [Required]
        public string VersionMask { get; set; }

        [JsonProperty(Order = 3)]
        [Required]
        public string CurrentVersion { get; set; }
    }
}
