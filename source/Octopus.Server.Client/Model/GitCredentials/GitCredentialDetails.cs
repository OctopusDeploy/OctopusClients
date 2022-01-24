using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.GitCredentials
{
    public enum GitCredentialDetailsType
    {
        UsernamePassword
    }
    
    public abstract class GitCredentialDetails 
    {
        public abstract GitCredentialDetailsType Type { get; }
    }

    public class UsernamePasswordGitCredentialDetails : GitCredentialDetails
    {
        public override GitCredentialDetailsType Type { get; } = GitCredentialDetailsType.UsernamePassword;

        [Writeable]
        public string Username { get; set; }

        [Writeable]
        public SensitiveValue Password { get; set; } = new SensitiveValue();
    }
}