#nullable enable
using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class GoogleContainerRegistryFeedResource : DockerFeedResource
    {
        public override FeedType FeedType => FeedType.GoogleContainerRegistry;
        
        [Writeable]
        public GoogleOidcFeedAuthentication? OidcAuthentication { get; set; }
        
        public class GoogleOidcFeedAuthentication : IOidcFeedAuthentication
        {
            
            public string? Audience { get; set; }
            public IEnumerable<string> SubjectKeys { get; set; } = [];
        }
    }
}