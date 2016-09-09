using System;
using System.ComponentModel.DataAnnotations;

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
        public string ManagementEndpoint { get; set; }
    }
}