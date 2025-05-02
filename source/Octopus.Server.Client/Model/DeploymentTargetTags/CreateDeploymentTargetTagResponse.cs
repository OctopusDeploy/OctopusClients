using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.DeploymentTargetTags;

public class CreateDeploymentTargetTagResponse
{
    [Required]
    public string Tag { get; set; }
        
    [Required]
    public string SpaceId { get; set; }    
}