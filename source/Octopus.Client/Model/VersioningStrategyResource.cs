using System;

namespace Octopus.Client.Model
{
    public class VersioningStrategyResource
    {
        public string Template { get; set; }
        
        public DeploymentActionPackageResource DonorPackage { get; set; }
        
        // This property is maintained for backward-compatibility
        [Obsolete("Replaced by DonorPackage")]
        public string DonorPackageStepId { get; set; }
    }
}