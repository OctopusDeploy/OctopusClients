using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class CommunityActionTemplateResource : IResource
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
        public string Website { get; set; }
        public string HistoryUrl { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IDictionary<string, PropertyValueResource> Properties { get; } = new Dictionary<string, PropertyValueResource>(StringComparer.OrdinalIgnoreCase);

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IList<ActionTemplateParameterResource> Parameters { get; } = new List<ActionTemplateParameterResource>();

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string StepPackageVersion { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object Inputs { get; set; }

        public LinkCollection Links { get; set; }        
    }
}