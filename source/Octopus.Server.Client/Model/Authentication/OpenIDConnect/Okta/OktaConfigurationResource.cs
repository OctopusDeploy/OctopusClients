#nullable enable
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
        public string? RoleClaimType { get; set; }

        [DisplayName("Username Claim Type")]
        [Description("Tell Octopus how to find the value for a new user's Octopus Username in the Okta token, while Require Email Verified Claim is off. Defaults to \"preferred_username\" if left blank.")]
        [Writeable]
        public string? UsernameClaimType { get; set; }

        [DisplayName("Require Email Verified Claim")]
        [Description("Reject a sign-in unless the security token has an email address and an email_verified claim. Only turn this off if your identity provider doesn't send both. While it's on, a new user's Username is their verified email address rather than the Username Claim Type's value")]
        [Writeable]
        public bool? RequireEmailVerifiedClaim { get; set; }
    }
}
