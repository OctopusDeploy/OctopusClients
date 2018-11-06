using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers
{
    public class DeploymentTargetFilterResource : TriggerFilterResource
    {
        public override TriggerFilterType FilterType => TriggerFilterType.DeploymentTargetFilter;

        [Writeable]
        public ReferenceCollection EnvironmentIds { get; set; } = new ReferenceCollection();
        [Writeable]
        public ReferenceCollection Roles { get; set; } = new ReferenceCollection();
        [Writeable]
        public ReferenceCollection EventGroups { get; set; } = new ReferenceCollection();
        [Writeable]
        public ReferenceCollection EventCategories { get; set; } = new ReferenceCollection();
    }
}