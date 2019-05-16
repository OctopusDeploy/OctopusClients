using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class ActionTemplateResource : Resource, INamedResource, IHaveSpaceResource
    {
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
        public IDictionary<string, PropertyValueResource> Properties { get; } = new Dictionary<string, PropertyValueResource>();

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public PackageReferenceCollection Packages { get; } = new PackageReferenceCollection(); 

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IList<ActionTemplateParameterResource> Parameters { get; } = new List<ActionTemplateParameterResource>();

        public string SpaceId { get; set; }
    }
}