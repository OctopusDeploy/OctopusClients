using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model
{
    public class ActionsUpdateResource : IResource
    {
        public ActionsUpdateResource()
        {
            ActionIdsByProcessId = new Dictionary<string, string[]>();
            DefaultPropertyValues = new Dictionary<string, PropertyValueResource>(StringComparer.OrdinalIgnoreCase);
            Links = new LinkCollection();
        }

        public string Id { get; set; }
        [Required]
        public int Version { get; set; }
        [Required]
        public IDictionary<string, string[]> ActionIdsByProcessId { get; set; }
        public IDictionary<string, PropertyValueResource> DefaultPropertyValues { get; set; }
        public LinkCollection Links { get; set; }
    }
}
