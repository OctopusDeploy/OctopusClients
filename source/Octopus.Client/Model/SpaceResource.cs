using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class SpaceResource : Resource
    {
        public SpaceResource()
        {
            SpaceManagersTeams = new ReferenceCollection();
            UsersToBeAddedToSpaceMangersTeam = new ReferenceCollection();
        }

        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string Description { get; set; }

        [Writeable]
        public bool IsDefault { get; set; }

        public bool TaskQueueStopped { get; set; }

        [Writeable]
        public ReferenceCollection SpaceManagersTeams { get; set; }
        
        [Writeable]
        public ReferenceCollection UsersToBeAddedToSpaceMangersTeam { get; set; }
    }
}
