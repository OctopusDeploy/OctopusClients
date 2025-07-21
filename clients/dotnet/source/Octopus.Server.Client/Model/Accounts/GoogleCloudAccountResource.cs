using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Accounts
{
    public class GoogleCloudAccountResource : AccountResource
    {
        public override AccountType AccountType => AccountType.GoogleCloudAccount;

        [Trim, Writeable, Required]
        public SensitiveValue JsonKey { get; set; }
    }
}