#nullable enable
using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class AzureContainerRegistryFeedResource : DockerFeedResource
    {
        public override FeedType FeedType => FeedType.AzureContainerRegistry;
        
        [Writeable]
        public AcrOidcFeedAuthentication? OidcAuthentication { get; set; }
    }
    public class AcrOidcFeedAuthentication : IOidcFeedAuthentication
    {
        public AcrOidcFeedAuthentication(string clientId, string tenantId)
        {
            ClientId = clientId;
            TenantId = tenantId;
        }

        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string? SessionDuration { get; set; } = "3600";
        public string? Audience { get; set; }
        public IEnumerable<string> SubjectKeys { get; set; } = [];
    }
}