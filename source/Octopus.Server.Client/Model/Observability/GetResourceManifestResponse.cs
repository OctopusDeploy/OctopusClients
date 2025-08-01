using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability;

public class GetResourceManifestResponse
{
    [Required]
    public string LiveManifest { get; set; }

    public string DesiredManifest { get; set; }

    public LiveResourceDiff Diff { get; set; }
}