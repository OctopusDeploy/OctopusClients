using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public enum VersionControlCredentialsType
    {
        Anonymous,
        UsernamePassword
    }
    
    public abstract class VersionControlCredentialsResource
    {
        public abstract VersionControlCredentialsType Type { get; }
    }

    public class AnonymousVersionControlCredentialsResource : VersionControlCredentialsResource
    {
        public override VersionControlCredentialsType Type { get; } = VersionControlCredentialsType.Anonymous;
    }

    public class UsernamePasswordVersionControlCredentialsResource : VersionControlCredentialsResource
    {
        public override VersionControlCredentialsType Type { get; } = VersionControlCredentialsType.UsernamePassword;

        [Writeable]
        public string Username { get; set; }
        [Writeable]
        public SensitiveValue Password { get; set; } = new SensitiveValue();
    }
}