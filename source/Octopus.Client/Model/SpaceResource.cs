using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class SpaceResource : Resource
    {
        public SpaceResource()
        {
            OwnerTeams = new ReferenceCollection();
            OwnerUsers = new ReferenceCollection();
        }

        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string Description { get; set; }

        [Writeable]
        public bool IsDefault { get; set; }

        public bool TaskQueueStopped { get; set; }

        [Writeable]
        public ReferenceCollection OwnerTeams { get; set; }
        
        [Writeable]
        public ReferenceCollection OwnerUsers { get; set; }
    }
}
