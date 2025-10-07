using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class AuthenticationConfigResource : IResource
    {
        public AuthenticationConfigResource()
        {
            Id = "authentication";
            TrustedRedirectUrls = new string[0];
        }

        public string Id { get; set; }

        public LinkCollection Links { get; set; }

        [Writeable]
        public bool AutoLoginEnabled { get; set; }

        [Writeable]
        public bool IsSelfServiceIdentityEditingEnabled { get; set; }

        [Writeable]
        public string[] TrustedRedirectUrls { get; set; }

        [Writeable]
        public string CookieDomain { get; set; }

        [Writeable]
        public int SessionTimeoutInSeconds { get; set; }

        [Writeable]
        public int MaximumSessionDurationInSeconds { get; set; }

        [Writeable]
        public bool RememberMeEnabled { get; set; }

        [Writeable]
        public bool UserApiKeysEnabled { get; set; }
        
        // Servers before 2025.4 do not have this property
        [Writeable]
        public int? ApiKeyDefaultExpiryDays { get; set; } 

        // Servers before 2025.4 do not have this property
        [Writeable]
        public int? ApiKeyMaxExpiryDays { get; set; }

        [Writeable]
        public OidcConfigResource OidcConfigurationSettings { get; set; }
    }
    
    public class OidcConfigResource
    {
        public OidcConfigResource()
        {
        }

        public OidcConfigResource(string oidcIssuerUrl)
        {
            OidcIssuerUrl = oidcIssuerUrl;
        }

        [Writeable]
        public string OidcIssuerUrl { get; set; }
    }
}
