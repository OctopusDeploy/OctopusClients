using System.ComponentModel;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Authentication.OpenIDConnect.Okta
{
    [Description("Sign in to your Octopus Server with Okta. [Learn more](https://g.octopushq.com/AuthOkta).")]
    public class OktaConfigurationResource : OpenIDConnectConfigurationResource
    {
        public OktaConfigurationResource()
        {
            Id = "authentication-od";
        }

        [DisplayName("Role Claim Type")]
        [Description("Tell Octopus how to find the roles in the security token from Okta")]
        [Writeable]
        public string RoleClaimType { get; set; }

        [DisplayName("Username Claim Type")]
        [Description("Tell Octopus how to find the value for the Octopus Username in the Okta token. Defaults to \"preferred_username\" if left blank.")]
        [Writeable]
        public string UsernameClaimType { get; set; }
    }
}
