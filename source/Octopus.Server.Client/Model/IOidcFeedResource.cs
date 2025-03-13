#nullable enable

namespace Octopus.Client.Model;

public interface IOidcFeedResource
{
    public OidcFeedAuthentication? OidcAuthentication { get; set; }
}