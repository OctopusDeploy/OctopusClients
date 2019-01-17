using Octopus.Client.Extensibility.Attributes;
using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model.Migrations
{
    public class MigrationPartialExportResource : Resource, IHaveSpaceResource
    {
        [Writeable]
        public string PackageId { get; set; }
        [Writeable]
        public string SpaceId { get; set; }
        [Writeable]
        public string PackageVersion { get; set; }
        [Writeable]
        public string Password { get; set; }
        [Writeable]
        public List<string> Projects { get; set; }
        [Writeable]
        public bool IgnoreCertificates { get; set; }
        [Writeable]
        public bool IgnoreMachines { get; set; }
        [Writeable]
        public bool IgnoreDeployments { get; set; }
        [Writeable]
        public bool IgnoreTenants { get; set; }
        [Writeable]
        public bool IncludeTaskLogs { get; set; }
        [Writeable]
        public bool EncryptPackage { get; set; }
        [Writeable]
        public string DestinationApiKey { get; set; }
        [Writeable]
        public string DestinationPackageFeed { get; set; }
        [Writeable]
        public string DestinationSpaceId { get; set; }
        [Writeable]
        public string SuccessCallbackUri { get; set; }
        [Writeable]
        public string FailureCallbackUri { get; set; }
        public string TaskId { get; set; }
    }
}
