using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class ReleaseTemplateResource : Resource
    {
        public string DeploymentProcessId { get; set; }
        public string LastReleaseVersion { get; set; }
        public string NextVersionIncrement { get; set; }
        public string VersioningPackageStepName { get; set; }
        public IList<ReleaseTemplatePackage> Packages { get; set; }
    }
}