namespace Octopus.Client.Model
{
    public class ProjectTriggerMachineFilterResource : IProjectTriggerFilterResource
    {
        public ReferenceCollection EnvironmentIds { get; } = new ReferenceCollection();
        public ReferenceCollection Roles { get; } = new ReferenceCollection();

        public ReferenceCollection EventGroups { get; } = new ReferenceCollection();

        public ReferenceCollection EventCategories { get; } = new ReferenceCollection();
    }
}