using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.DeploymentTargetTags;

public class GetDeploymentTargetTagByTagResponse
{
    [Required]
    public string Tag { get; set; }
        
    [Required]
    public string SpaceId { get; set; }    
}