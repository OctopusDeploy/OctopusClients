using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class VersionControlBranchResource : Resource
    {
        public VersionControlBranchResource(string name)
        {
            Name = name;
            Links = new LinkCollection();
        }

        public string Name { get; }
    }
}