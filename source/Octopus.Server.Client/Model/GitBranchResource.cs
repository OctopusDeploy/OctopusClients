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

        public string CanonicalName { get; }
        public string Name { get; }
    }
    
    public class GitTagResource : Resource
    {
        public GitTagResource(string name)
        {
            Name = name;
            Links = new LinkCollection();
        }

        public string CanonicalName { get; }
        public string Name { get; }
    }
    
    public class GitCommitResource : Resource
    {
        public GitCommitResource(string name)
        {
            Name = name;
            Links = new LinkCollection();
        }

        public string CanonicalName { get; }
        public string Name { get; }
    }
}