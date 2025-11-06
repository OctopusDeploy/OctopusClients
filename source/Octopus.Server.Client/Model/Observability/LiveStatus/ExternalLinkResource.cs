using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.LiveStatus;

public class ExternalLinkResource
{
    [Required]
    public string Label { get; set; }
    [Required]
    public string Uri { get; set; }
}
