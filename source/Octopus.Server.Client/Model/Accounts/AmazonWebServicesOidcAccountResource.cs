using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Accounts
{
    public class AmazonWebServicesOidcAccountResource : AccountResource
    {
        public override AccountType AccountType => AccountType.AmazonWebServicesOidcAccount;

        [Trim]
        [Writeable]
        public string RoleArn { get; set; }

        [Writeable]
        public string SessionDuration { get; set; }

        [Trim]
        [Writeable]
        public string Region { get; set; }

        [Writeable]
        public string[] DeploymentSubjectKeys { get; set; }

        [Writeable]
        public string[] HealthCheckSubjectKeys { get; set; }

        [Writeable]
        public string[] AccountTestSubjectKeys { get; set; }

        [Writeable]
        public Dictionary<string, string> CustomClaims { get; set; } = new Dictionary<string, string>();
    }
}
