using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.WebPortalConfiguration;

public class WebPortalLoggingResource
{
    [Writeable]
    public string[] TrustedProxies { get; set; }
}