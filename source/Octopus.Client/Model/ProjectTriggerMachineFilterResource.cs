namespace Octopus.Client.Model
{
    public class ProjectTriggerMachineFilterResource : IProjectTriggerFilterResource
    {
        public ReferenceCollection EnvironmentIds { get; }
        public ReferenceCollection Roles { get; }

        public ReferenceCollection EventGroups { get; }

        public ReferenceCollection EventCategories { get; }

    }
}