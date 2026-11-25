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
        [Description("Tell Octopus how to find the value for a new user's Octopus Username in the OIDC token, while Require Email Verified Claim and Require Email Domain Verified Claim are off. Defaults to \"preferred_username\".")]
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

        [DisplayName("Require Email Verified Claim")]
        [Description("Reject a sign-in unless the security token has an email address and an email_verified claim. Only turn this off if your identity provider doesn't send both. While it's on, a new user's Username is their verified email address rather than the Username Claim Type's value")]
        [Writeable]
        public bool? RequireEmailVerifiedClaim { get; set; }

        [DisplayName("Require Email Domain Verified Claim")]
        [Description("Reject a sign-in unless the security token has an email address and an xms_edov claim. Turn this on for Microsoft Entra ID, and turn off Require Email Verified Claim, which Entra ID doesn't send. Add email and xms_edov as optional claims to your app registration's ID token. While it's on, a new user's Username is their verified email address rather than the Username Claim Type's value")]
        [Writeable]
        public bool? RequireEmailDomainVerifiedClaim { get; set; }
    }
}
