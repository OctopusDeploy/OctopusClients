using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class GitBranchResource : Resource
    {
        public GitBranchResource(string name)
        {
            Name = name;
            Links = new LinkCollection();
        }

        public string Name { get; }
    }
}