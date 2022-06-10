using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public abstract class ValidatedGitReferenceResource : Resource
    {
        public ValidatedGitReferenceResource(string name, string canonicalName)
        {
            Name = name;
            CanonicalName = canonicalName;
            Links = new LinkCollection();
        }

        public string CanonicalName { get; }
        public string Name { get; }
    }
    public class GitBranchResource : ValidatedGitReferenceResource
    {
        public GitBranchResource(string name, string canonicalName) : base(name, canonicalName)
        {
        }
        
        public GitBranchResource(string name) : base(name, name)
        {
        }
    }
    
    public class GitTagResource : ValidatedGitReferenceResource
    {
        public GitTagResource(string name, string canonicalName) : base(name, canonicalName)
        {
        }
        
        public GitTagResource(string name) : base(name, name)
        {
        }
    }
    
    public class GitCommitResource : ValidatedGitReferenceResource
    {
        public GitCommitResource(string name, string canonicalName) : base(name, canonicalName)
        {
        }
        
        public GitCommitResource(string name) : base(name, name)
        {
        }
    }
}