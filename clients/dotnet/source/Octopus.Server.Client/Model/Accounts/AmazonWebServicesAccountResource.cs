using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Accounts
{
    public class AmazonWebServicesAccountResource : AccountResource
    {
        public override AccountType AccountType => AccountType.AmazonWebServicesAccount;

        [Trim]
        [Writeable]
        [Required]
        public string AccessKey { get; set; }

        [Trim]
        [Writeable]
        [Required]
        public SensitiveValue SecretKey { get; set; }
    }
}