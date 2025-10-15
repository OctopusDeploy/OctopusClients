#nullable enable
using System.ComponentModel;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Authentication.OpenIDConnect.GenericOidc
{
    // TODO: Correct 'learn more' link
    [Description("Sign in to your Octopus Server with OIDC. [Learn more](https://g.octopushq.com/AuthOidc).")]
    public class GenericOidcConfigurationResource : OpenIDConnectConfigurationResource
    {
        public GenericOidcConfigurationResource()
        {
            Id = "authentication-generic-oidc";
        }

        [DisplayName("Role Claim Type")]
        [Description("Tell Octopus how to find the roles in the security token from the OIDC provider")]
        [Writeable]
        public string? RoleClaimType { get; set; }

        [DisplayName("Username Claim Type")]
        [Description("Tell Octopus how to find the value for the Octopus Username in the OIDC token. Defaults to \"preferred_username\" if left blank.")]
        [Writeable]
        public string? UsernameClaimType { get; set; }
    }
}
