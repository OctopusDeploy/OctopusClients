using Octopus.Client.Model;

namespace Octopus.Client;

public interface IOctopusServerRootResourceCache
{
    RootResource CachedRootResource { get; set; }
}