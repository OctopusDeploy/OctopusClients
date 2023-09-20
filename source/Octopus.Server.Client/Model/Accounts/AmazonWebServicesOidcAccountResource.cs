using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Accounts
{
    public class AmazonWebServicesOidcAccountResource : AccountResource
    {
        public override AccountType AccountType => AccountType.AmazonWebServicesOidcAccount;

        [Trim]
        [Writeable]
        public string RoleArn { get; set; }

        [Trim]
        [Writeable]
        [Required]
        public string SessionDuration { get; set; }
        
        [Trim]
        [Writeable]
        [Required]
        public string Audience { get; set; }
    }
}