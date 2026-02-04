#nullable enable
using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers
{
    public class RunRunbookActionResource : TriggerActionResource
    {
        public override TriggerActionType ActionType => TriggerActionType.RunRunbook;
        
        public RunRunbookActionResource()
        {
            EnvironmentIds = new ReferenceCollection();
            TenantTags = new ReferenceCollection();
            TenantIds = new ReferenceCollection();
        }

        [Writeable]
        public string? RunbookId { get; set; }

        [Writeable]
        public List<TagCanonicalIdOrName>? RunbookTags { get; set; }
        
        [Writeable]
        public ReferenceCollection EnvironmentIds { get; set; }
        
        [Writeable]
        public ReferenceCollection TenantIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantTags { get; set; }
        
    }
}
