using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Accounts
{
    public class GenericOidcAccountResource : AccountResource
    {
        public override AccountType AccountType => AccountType.GenericOidcAccount;

        [Trim]
        [Writeable]
        public string Audience { get; set; }

        [Writeable]
        public string[] DeploymentSubjectKeys { get; set; }

        [Writeable]
        public string[] HealthCheckSubjectKeys { get; set; }

        [Writeable]
        public string[] AccountTestSubjectKeys { get; set; }
    }
}