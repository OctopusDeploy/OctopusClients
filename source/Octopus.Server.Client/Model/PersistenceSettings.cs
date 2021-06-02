using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public enum PersistenceSettingsType
    {
        Database,
        VersionControlled
    }
    
    public abstract class PersistenceSettingsResource
    {
        public abstract PersistenceSettingsType Type { get; }
    }

    public class DatabasePersistenceSettingsResource : PersistenceSettingsResource
    {
        public override PersistenceSettingsType Type { get; } = PersistenceSettingsType.Database;
    }

    public class VersionControlSettingsResource : PersistenceSettingsResource
    {
        public override PersistenceSettingsType Type { get; } = PersistenceSettingsType.VersionControlled;
        [Writeable]
        public string Url { get; set; }
        [Writeable]
        public VersionControlCredentialsResource Credentials { get; set; } =
            new AnonymousVersionControlCredentialsResource();
        [Writeable]
        public string DefaultBranch { get; set; }
        [Writeable]
        public string BasePath { get; set; }
    }
}