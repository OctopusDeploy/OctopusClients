using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.WebPortalConfiguration;

public class XOptionsResource
{
    public const string XFrameAllowFromDescription = "(Deprecated) A uri to provide in the X-Frame-Option http header in conjunction with the ALLOW-FROM value.";
    public const string XFrameOptionsDescription = "Provide in the X-Frame-Option http header a directive such as sameorigin or deny.";


    [Writeable]
    public string XFrameOptionAllowFrom { get; set; }
        
    [Writeable]
    public string XFrameOptions { get; set; }
}