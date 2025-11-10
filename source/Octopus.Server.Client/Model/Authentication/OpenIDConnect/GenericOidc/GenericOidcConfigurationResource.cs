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
        [Description("Tell Octopus how to find the roles in the security token from the OIDC provider. Defaults to \"groups\".")]
        [Writeable]
        public string? RoleClaimType { get; set; }

        [DisplayName("Username Claim Type")]
        [Description("Tell Octopus how to find the value for the Octopus Username in the OIDC token. Defaults to \"preferred_username\".")]
        [Writeable]
        public string? UsernameClaimType { get; set; }

        [DisplayName("Resource")]
        [Description("0Auth 2.0 resource parameter. Typically the absolute URI of the target resource where the access token will be used. Optional.")]
        [Writeable]
        public string? Resource { get; set; }

        [DisplayName("Scopes")]
        [Description("OAuth 2.0 scopes to request during authentication. Must include 'openid', 'profile', and 'email'.")]
        [Writeable]
        public string[]? Scopes { get; set; }
        
        [DisplayName("Display Name")]
        [Description("The name to show on the login page. May contain only be letters, numbers, hyphens or spaces. Defaults to \"OpenID Connect\" if left blank.")]
        [Writeable]
        public string? DisplayName { get; set; }
    }
}
