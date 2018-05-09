using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class CertificateUsageResource
    {
        public ICollection<CertificateVariableUsageResource<ProjectResource>> ProjectUsages { get; set; }

        public ICollection<CertificateVariableUsageResource<LibraryVariableSetResource>> LibraryVariableSetUsages { get; set; }

        public ICollection<CertificateVariableUsageResource<TenantResource>> TenantUsages { get; set; }

        public class CertificateVariableUsageResource<T>
        {
            public bool IsMissingScopedPermissions { get; set; }
            public string MissingId { get; set;}
            public T Owner { get; set; }
        }
    }
}