using System;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Accounts
{
    public class AzureSubscriptionAccountResource : AccountResource
    {
        public AzureSubscriptionAccountResource()
        {
            CertificateBytes = new SensitiveValue();
        }

        public override AccountType AccountType
        {
            get { return AccountType.AzureSubscription; }
        }

        [Trim]
        [Writeable]
        [Required(ErrorMessage = "Please provide an Azure subscription ID.")]
        public string SubscriptionNumber { get; set; }

        [Trim]
        [Writeable]
        public SensitiveValue CertificateBytes { get; set; }

        [Trim]
        [Writeable]
        public string CertificateThumbprint { get; set; }

        [Trim]
        [Writeable]
        public string AzureEnvironment { get; set; }

        [Trim]
        [Writeable]
        public string ServiceManagementEndpointBaseUri { get; set; }

        public class WebSiteResource
        {
            public string Name { get; }
            public string WebSpace { get; set; }
            public string ResourceGroup { get; set; }
        }
    }
}