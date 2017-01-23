using System;

namespace Octopus.Client.Model
{
    public class CertificateResource : Resource, INamedResource
    {
        public CertificateResource()
        {
            EnvironmentIds = new ReferenceCollection();            
            TenantIds = new ReferenceCollection();
            TenantTags = new ReferenceCollection();
        }

        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string Notes { get; set; }

        [WriteableOnCreate]
        public SensitiveValue CertificateData { get; set; }

        [WriteableOnCreate]
        public SensitiveValue Password { get; set; }

        [Writeable]
        public ReferenceCollection EnvironmentIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantTags { get; set; }

        public CertificateFormat CertificateDataFormat { get; set; }  

        public DateTimeOffset? Archived { get; set; } 

        public string SubjectDistinguishedName { get; set; }

        public string SubjectCommonName { get; set; }

        public string SubjectOrganization { get; set; }

        public string IssuerDistinguishedName { get; set; }

        public string IssuerCommonName { get; set; }

        public string IssuerOrganization { get; set; }

        public bool SelfSigned { get; set; }

        public string Thumbprint { get; set; }

        public DateTimeOffset NotAfter { get; set; } 

        public DateTimeOffset NotBefore { get; set; } 

        public bool IsExpired { get; set; }

        public bool HasPrivateKey { get; set; }

        public int Version { get; set; }

        public string SerialNumber { get; set; }

        public string SignatureAlgorithmName { get; set; }

    }
}