using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class SpaceResource : Resource
    {
        public SpaceResource()
        {
            Owners = new ReferenceCollection();
        }

        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string Description { get; set; }

        [Writeable]
        public bool IsDefault { get; set; }

        [Writeable]
        public ReferenceCollection Owners { get; set; }
    }
}
