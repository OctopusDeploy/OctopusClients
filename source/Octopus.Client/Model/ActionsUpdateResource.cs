using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class ActionsUpdateResource : IResource
    {
        public string Id { get; set; }
        [Required] 
        public int Version { get; set; }
        public IDictionary<string, PropertyValueResource> DefaultPropertyValues { get; set; } = new Dictionary<string, PropertyValueResource>(StringComparer.OrdinalIgnoreCase);
        public IDictionary<string, PropertyValueResource> Overrides { get; set; } = new Dictionary<string, PropertyValueResource>(StringComparer.OrdinalIgnoreCase);
        public LinkCollection Links { get; set; } = new LinkCollection();

        [Obsolete] 
        public IDictionary<string, string[]> ActionIdsByProcessId { get; set; } = new Dictionary<string, string[]>();

        [Required]
        public ActionsUpdateProcessResource[] ActionsToUpdate { get; set; } = { };
    }

    public class ActionsUpdateProcessResource : IResource
    {
        public ProcessType ProcessType { get; set; }
        public string ProcessId { get; set; }
        public string[] ActionIds { get; set; }

        public string Id { get; set; }
        public LinkCollection Links { get; set; }
    }
}