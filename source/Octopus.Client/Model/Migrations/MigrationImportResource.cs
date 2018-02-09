using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Migrations
{
    public class MigrationImportResource : Resource
    {
        [Writeable]
        public string PackageId { get; set; }
        [Writeable]
        public string PackageVersion { get; set; }
        [Writeable]
        public string Password { get; set; }
        [Writeable]
        public bool IsEncryptedPackage { get; set; }
        [Writeable]
        public bool IsDryRun { get; set; }
        [Writeable]
        public bool OverwriteExisting { get; set; }
        [Writeable]
        public bool DeletePackageOnCompletion { get; set; }
        [Writeable]
        public string SuccessCallbackUri { get; set; }
        [Writeable]
        public string FailureCallbackUri { get; set; }
        public string TaskId { get; set; }
    }
}
