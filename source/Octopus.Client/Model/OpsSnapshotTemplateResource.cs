using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class OpsSnapshotTemplateResource : Resource
    {
        public string OpsStepsId { get; set; }
        public string LastReleaseVersion { get; set; }
        public string NextVersionIncrement { get; set; }
        public string VersioningPackageStepName { get; set; }
        public string VersioningPackageReferenceName { get; set; }
        public IList<ReleaseTemplatePackage> Packages { get; set; }
    }
}