using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public enum ProjectGitCredentialType
    {
        Anonymous,
        UsernamePassword,
        Reference,
        GitHub
    }

    public abstract class ProjectGitCredentialResource
    {
        public abstract ProjectGitCredentialType Type { get; }
    }

    public class AnonymousProjectGitCredentialResource : ProjectGitCredentialResource
    {
        public override ProjectGitCredentialType Type { get; } = ProjectGitCredentialType.Anonymous;
    }

    public class UsernamePasswordProjectGitCredentialResource : ProjectGitCredentialResource
    {
        public override ProjectGitCredentialType Type { get; } = ProjectGitCredentialType.UsernamePassword;

        [Writeable]
        public string Username { get; set; }
        [Writeable]
        public SensitiveValue Password { get; set; } = new SensitiveValue();
    }

    public class ReferenceProjectGitCredentialResource : ProjectGitCredentialResource
    {
        public override ProjectGitCredentialType Type { get; } = ProjectGitCredentialType.Reference;

        [Writeable]
        public string Id { get; set; }
    }
    
    public class GitHubProjectGitCredentialResource : ProjectGitCredentialResource
    {
        public override ProjectGitCredentialType Type { get; } = ProjectGitCredentialType.GitHub;

        [Writeable]
        public string Id { get; set; }
    }
}