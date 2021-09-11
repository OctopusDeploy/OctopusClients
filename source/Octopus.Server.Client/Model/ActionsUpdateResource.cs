using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class ActionsUpdateResource : IResource
    {
        public ActionsUpdateResource()
        {
#pragma warning disable 618
            ActionIdsByProcessId = new Dictionary<string, string[]>();
#pragma warning restore 618
            DefaultPropertyValues = new Dictionary<string, PropertyValueResource>(StringComparer.OrdinalIgnoreCase);
            Overrides = new Dictionary<string, PropertyValueResource>(StringComparer.OrdinalIgnoreCase);
            Links = new LinkCollection();
        }

        public string Id { get; set; }
        [Required]
        public int Version { get; set; }
        public IDictionary<string, PropertyValueResource> DefaultPropertyValues { get; set; }
        public IDictionary<string, PropertyValueResource> Overrides { get; set; }
        public LinkCollection Links { get; set; }
        
        [Obsolete("Use" + nameof(ActionsToUpdate) + " instead")]
        public IDictionary<string, string[]> ActionIdsByProcessId { get; set; }
        
        [Required]
        public ActionsUpdateProcessResource[] ActionsToUpdate { get; set; } = {};
    }

    public class ActionsUpdateProcessResource : IResource
    {
        public ProcessType ProcessType { get; set; }
        public string ProcessId { get; set; }
        public string[] ActionIds { get; set; }
        
        public string Id { get; set; }
        public LinkCollection Links { get; set; }
        public string GitRef { get; set; }
    }
}