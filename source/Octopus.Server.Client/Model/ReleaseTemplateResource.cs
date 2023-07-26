using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class ReleaseTemplateResource : ReleaseTemplateBaseResource
    {
        public string DeploymentProcessId { get; set; }
        public string LastReleaseVersion { get; set; }
        public string NextVersionIncrement { get; set; }
        public string VersioningPackageStepName { get; set; }
        public string VersioningPackageReferenceName { get; set; }
    }

    public class ReleaseTemplateBaseResource : Resource
    {                                                                
        public IList<ReleaseTemplatePackage> Packages { get; set; }
        public IList<ReleaseTemplateGitResource> GitResources { get; set; }
    }
}