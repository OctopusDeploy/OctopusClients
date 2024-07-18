using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.DeploymentTargetTags;

public class GetDeploymentTargetTagByTagRequest
{
    /// <summary>
    /// The ID of the space containing the resource(s).
    /// </summary>
    [Required]
    public string SpaceId { get; set; } = null!;
    
    /// <summary>
    /// ID or Slug of the DeploymentTargetTag
    /// </summary>
    [Required]
    public string Tag { get; set; }        
}