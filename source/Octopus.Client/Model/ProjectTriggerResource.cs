using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class ProjectTriggerResource : Resource, INamedResource
    {
        readonly IDictionary<string, PropertyValueResource> properties = new Dictionary<string, PropertyValueResource>(StringComparer.OrdinalIgnoreCase);

        public ProjectTriggerResource()
        {
        }

        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string ProjectId { get; set; }

        [Writeable]
        public ProjectTriggerType Type { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IDictionary<string, PropertyValueResource> Properties
        {
            get { return properties; }
        }
    }
}
