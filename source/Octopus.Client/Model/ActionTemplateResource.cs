using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class ActionTemplateResource : Resource, INamedResource
    {
        readonly IDictionary<string, PropertyValueResource> properties = new Dictionary<string, PropertyValueResource>(StringComparer.OrdinalIgnoreCase);
        readonly IList<ActionTemplateParameterResource> parameters = new List<ActionTemplateParameterResource>();

        [Required(ErrorMessage = "Please provide a name for the template.")]
        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please provide an action type.")]
        [WriteableOnCreate]
        public string ActionType { get; set; }

        public int Version { get; set; }

        public string CommunityActionTemplateId { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IDictionary<string, PropertyValueResource> Properties
        {
            get { return properties; }
        }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IList<ActionTemplateParameterResource> Parameters
        {
            get { return parameters; }
        }
    }
}