using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public sealed class IdentityResource
    {
        public IdentityResource()
        {
            Claims = new Dictionary<string, IdentityClaimResource>();
        }

        public IdentityResource(string identityProviderName)
            : this()
        {
            IdentityProviderName = identityProviderName;
        }

        public string IdentityProviderName { get; set; }

        public Dictionary<string, IdentityClaimResource> Claims { get; }
    }
}