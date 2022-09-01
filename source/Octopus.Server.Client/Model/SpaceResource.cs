using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class SpaceResource : Resource, INamedResource
    {
        public SpaceResource()
        {
            SpaceManagersTeams = new ReferenceCollection();
            SpaceManagersTeamMembers = new ReferenceCollection();
        }

        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string Slug { get; set; }

        [Writeable]
        public string Description { get; set; }

        [Writeable]
        public bool IsDefault { get; set; }

        public bool IsPrivate { get; set; }

        public bool TaskQueueStopped { get; set; }

        [Writeable]
        public ReferenceCollection SpaceManagersTeams { get; set; }
        
        [Writeable]
        public ReferenceCollection SpaceManagersTeamMembers { get; set; }
		
        public string PrivateSpaceOwner { get; set; }
        
        public IconResource Icon { get; set; }
    }
}
