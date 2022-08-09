using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public abstract class ValidatedGitReferenceResource : Resource
    {
        public ValidatedGitReferenceResource(string canonicalName)
        {
            CanonicalName = canonicalName;
            Links = new LinkCollection();
        }

        public string CanonicalName { get; }
        public string Name { get; set; }
    }
    public class GitBranchResource : ValidatedGitReferenceResource
    {
        public bool IsProtected { get; }
    
        public GitBranchResource(string canonicalName, bool isProtected = false) : base(canonicalName)
        {
            IsProtected = isProtected;
        }
    }
    
    public class GitTagResource : ValidatedGitReferenceResource
    {
        public GitTagResource(string canonicalName) : base(canonicalName)
        {
        }
    }
    
    public class GitCommitResource : ValidatedGitReferenceResource
    {
        public GitCommitResource(string canonicalName) : base(canonicalName)
        {
        }
    }
}