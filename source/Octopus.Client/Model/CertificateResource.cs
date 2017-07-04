using System;
using System.Linq;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class CertificateResource : Resource, INamedResource
    {
        [JsonConstructor]
        protected CertificateResource()
        {
            EnvironmentIds = new ReferenceCollection();            
            TenantIds = new ReferenceCollection();
            TenantTags = new ReferenceCollection();
        }

        public CertificateResource(string name, string certificateData)
            :this()
        {
            Name = name;
            CertificateData = certificateData;
        }

        public CertificateResource(string name, string certificateData, string password)
            :this()
        {
            Name = name;
            CertificateData = certificateData;
            Password = password;
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

        // Nullable backing-field is to support backwards-compatibility
        TenantedDeploymentMode? tenantedDeploymentParticipation;

        [Writeable]
        public TenantedDeploymentMode TenantedDeploymentParticipation
        {
            set => tenantedDeploymentParticipation = value;
            
            get
            {
                if (tenantedDeploymentParticipation.HasValue)
                    return tenantedDeploymentParticipation.Value;

                // Responses from server versions before TenantedDeploymentParticipation was implemented will default
                // to pre-existing behaviour 
                return TenantIds.Any() || TenantTags.Any()
                    ? TenantedDeploymentMode.Tenanted
                    : TenantedDeploymentMode.Untenanted;
            }
        }

        [Writeable]
        public ReferenceCollection TenantIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantTags { get; set; }

        [JsonProperty]
        public DateTimeOffset? Archived { get; private set; } 

        [JsonProperty]
        public string ReplacedBy { get; private set; }

        [JsonProperty]
        public CertificateFormat CertificateDataFormat { get; private set; }  

        [JsonProperty]
        public string SubjectDistinguishedName { get; private set; }

        [JsonProperty]
        public string SubjectCommonName { get; private set; }

        [JsonProperty]
        public string SubjectOrganization { get; private set; }

        [JsonProperty]
        public string IssuerDistinguishedName { get; private set; }

        [JsonProperty]
        public string IssuerCommonName { get; private set; }

        [JsonProperty]
        public string IssuerOrganization { get; private set; }

        [JsonProperty]
        public bool SelfSigned { get; private set; }

        [JsonProperty]
        public string Thumbprint { get; private set; }

        [JsonProperty]
        public DateTimeOffset NotAfter { get; private set; } 

        [JsonProperty]
        public DateTimeOffset NotBefore { get; private set; } 

        [JsonProperty]
        public bool IsExpired { get; private set; }

        [JsonProperty]
        public bool HasPrivateKey { get; private set; }

        [JsonProperty]
        public int Version { get; private set; }

        [JsonProperty]
        public string SerialNumber { get; private set; }

        [JsonProperty]
        public string SignatureAlgorithmName { get; private set; }

    }
}