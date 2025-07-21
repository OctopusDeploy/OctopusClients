using Octopus.Client.Extensibility;

namespace Octopus.Client.Model.WebPortalConfiguration;

public class WebPortalConfigResource : IResource
{
    public WebPortalConfigResource()
    {
        Id = "webportal";
    }

    public string Id { get; set; }

    public LinkCollection Links { get; set; }

    //Not implemented yet
    //public string PublicBaseUrl { get; set; }

    public WebPortalSecurityResource Security { get; set; } = new();
    public WebPortalLoggingResource Logging { get; set; } = new();
}