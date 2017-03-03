using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model
{
    public class ActionsUpdateResource : IResource
    {
        public string Id { get; set; }
        [Required]
        public int Version { get; set; }
        [Required]
        public Dictionary<string, string[]> ActionIdsByProcessId { get; set; }
        public Dictionary<string, PropertyValueResource> DefaultPropertyValues { get; set; }
        public LinkCollection Links { get; set; }
    }
}
