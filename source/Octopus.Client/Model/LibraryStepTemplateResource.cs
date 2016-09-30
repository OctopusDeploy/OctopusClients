using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class LibraryStepTemplateResource : IResource
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IDictionary<string, PropertyValueResource> Properties { get; } = new Dictionary<string, PropertyValueResource>(StringComparer.OrdinalIgnoreCase);

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IList<ActionTemplateParameterResource> Parameters { get; } = new List<ActionTemplateParameterResource>();

        public LinkCollection Links { get; set; }
    }
}