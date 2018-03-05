using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers
{
    public abstract class ScopedDeploymentActionResource : TriggerActionResource
    {
        protected ScopedDeploymentActionResource()
        {
            TenantTags = new ReferenceCollection();
            TenantIds = new ReferenceCollection();
        }

        [Writeable]
        public string ChannelId { get; set; }

        [Writeable]
        public ReferenceCollection TenantIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantTags { get; set; }
    }
}
