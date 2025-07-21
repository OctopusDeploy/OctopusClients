using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class CertificateUsageResource : Resource
    {
        public ICollection<ProjectResource> ProjectUsages { get; set; } = new List<ProjectResource>();

        public ICollection<LibraryVariableSetResource> LibraryVariableSetUsages { get; set; } = new List<LibraryVariableSetResource>();

        public ICollection<TenantResource> TenantUsages { get; set; } = new List<TenantResource>();
        
        public ICollection<MachineResource>  DeploymentTargetUsages { get; set; } = new List<MachineResource>();
    }
}