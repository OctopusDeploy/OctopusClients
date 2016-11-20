namespace Octopus.Client.Model.Triggers
{
    public class MachineFilterResource : TriggerFilterResource
    {
        public override TriggerFilterType FilterType => TriggerFilterType.MachineFilter;
        public ReferenceCollection EnvironmentIds { get; } = new ReferenceCollection();
        public ReferenceCollection Roles { get; } = new ReferenceCollection();
        public ReferenceCollection EventGroups { get; } = new ReferenceCollection();
        public ReferenceCollection EventCategories { get; } = new ReferenceCollection();
    }
}