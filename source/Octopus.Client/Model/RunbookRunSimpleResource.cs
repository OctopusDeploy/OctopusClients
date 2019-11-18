using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RunbookRunSimpleResource : Resource, IHaveSpaceResource, ISupportSimpleRunbookRun
    {
        public RunbookRunSimpleResource()
        {
            FormValues = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        [WriteableOnCreate]
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public Dictionary<string, string> FormValues { get; set; }

        public string SpaceId { get; set; }

        [Required(ErrorMessage = "Please provide a target environment to run to.")]
        [WriteableOnCreate]
        public string EnvironmentId { get; set; }

        [WriteableOnCreate]
        public string TenantId { get; set; }
    }
}