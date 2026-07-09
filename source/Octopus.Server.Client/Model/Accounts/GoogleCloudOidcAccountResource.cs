using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Accounts
{
    public class GoogleCloudOidcAccountResource : AccountResource
    {
        public override AccountType AccountType => AccountType.GoogleCloudOidcAccount;

        [Trim]
        [Writeable]
        public string Audience { get; set; }

        [Writeable]
        public string TokenLifetimeSeconds { get; set; }

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
