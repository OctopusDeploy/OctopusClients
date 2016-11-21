namespace Octopus.Client.Model.Triggers
{
    public class MachineFilterResource : TriggerFilterResource
    {
        [Writeable]
        public override TriggerFilterType FilterType => TriggerFilterType.MachineFilter;
        [Writeable]
        public ReferenceCollection EnvironmentIds { get; } = new ReferenceCollection();
        [Writeable]
        public ReferenceCollection Roles { get; } = new ReferenceCollection();
        [Writeable]
        public ReferenceCollection EventGroups { get; } = new ReferenceCollection();
        [Writeable]
        public ReferenceCollection EventCategories { get; } = new ReferenceCollection();
    }
}