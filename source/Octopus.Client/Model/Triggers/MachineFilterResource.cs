namespace Octopus.Client.Model.Triggers
{
    public class MachineFilterResource : TriggerFilterResource
    {
        [Writeable]
        public override TriggerFilterType FilterType => TriggerFilterType.MachineFilter;
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