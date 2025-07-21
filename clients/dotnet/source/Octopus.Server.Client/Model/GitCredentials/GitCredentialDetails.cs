using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.GitCredentials
{
    public enum GitCredentialType
    {
        UsernamePassword
    }

    public abstract class GitCredentialDetails 
    {
        public abstract GitCredentialType Type { get; }
    }

    public class UsernamePasswordGitCredentialDetails : GitCredentialDetails
    {
        public override GitCredentialType Type { get; } = GitCredentialType.UsernamePassword;

        [Writeable]
        public string Username { get; set; }

        [Writeable]
        public SensitiveValue Password { get; set; } = new SensitiveValue();
    }
}