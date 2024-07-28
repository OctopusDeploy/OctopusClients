using System.Collections.Generic;
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

    public class GitPersistenceSettingsResource : PersistenceSettingsResource
    {
        public override PersistenceSettingsType Type { get; } = PersistenceSettingsType.VersionControlled;
        [Writeable]
        public string Url { get; set; }
        [Writeable]
        public ProjectGitCredentialResource Credentials { get; set; } =
            new AnonymousProjectGitCredentialResource();
        [Writeable]
        public string DefaultBranch { get; set; }
        [Writeable]
        public string BasePath { get; set; }
        [Writeable]
        public bool ProtectedDefaultBranch { get; set; }
        [Writeable]
        public List<string> ProtectedBranchNamePatterns { get; set; } = new();

        public GitPersistenceSettingsConversionStateResource ConversionState { get; set; }
    }
    
    public class GitPersistenceSettingsConversionStateResource
    {
        public bool VariablesAreInGit { get; set; }
        
        public bool RunbooksAreInGit { get; set; }
    }
}