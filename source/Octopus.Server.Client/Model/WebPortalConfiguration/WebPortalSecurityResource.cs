using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.WebPortalConfiguration;

public class WebPortalSecurityResource
{
    [Writeable]
    public string CorsWhitelist { get; set; }

    [Writeable]
    public string ReferrerPolicy { get; set; }

    [Writeable]
    public bool ContentSecurityPolicyEnabled { get; set; }
    
    [Writeable]
    public bool UnsafeScriptSourceEnabled { get; set; }

    [Writeable]
    public bool HttpStrictTransportSecurityEnabled { get; set; }

    [Writeable]
    [Required]
    public long HttpStrictTransportSecurityMaxAge { get; set; }

    public XOptionsResource XOptions { get; set; } = new XOptionsResource();
}