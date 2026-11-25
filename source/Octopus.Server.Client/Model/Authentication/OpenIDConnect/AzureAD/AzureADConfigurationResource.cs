#nullable enable
using System.ComponentModel;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Authentication.OpenIDConnect.AzureAD
{
    [Description("Sign in to your Octopus Server with Azure Active Directory. [Learn more](https://g.octopushq.com/AuthAzureAD).")]
    public class AzureADConfigurationResource : OpenIDConnectConfigurationResource
    {
        public AzureADConfigurationResource()
        {
            Id = "authentication-aad";
        }

        [DisplayName("Role Claim Type")]
        [Description("Tell Octopus how to find the roles/groups in the security token from Azure Active Directory (usually \"roles\" or \"groups\")")]
        [Writeable]
        public string? RoleClaimType { get; set; }

        [DisplayName("Enable SCIM")]
        [Description("Enable System for Cross-domain Identity Management (SCIM) to allow Azure AD to manage users and groups in Octopus Deploy. Cannot be enabled if 'Allow Auto User Creation' is enabled.")]
        [Category("SCIM")]
        [Writeable]
        public bool? ScimEnabled { get; set; }

        [DisplayName("Require Email Domain Verified Claim")]
        [Description("Reject a sign-in unless the security token has an email address and an xms_edov claim. Add email and xms_edov as optional claims to your app registration's ID token. While it's on, a new user's Username is their verified email address rather than their UPN")]
        [Writeable]
        public bool? RequireEmailDomainVerifiedClaim { get; set; }
    }
}
