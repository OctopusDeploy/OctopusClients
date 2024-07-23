using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model.DeploymentTargetTags;

public class CreateDeploymentTargetTagCommand : IHaveSpaceResource
{
    /// <summary>
    /// The name or tag of the DeploymentTargetTag
    /// </summary>
    [Required(ErrorMessage = "Deployment Target Tag must have a name.")]
    [MaxLength(200, ErrorMessage = "Tag must be 200 characters or less.")]
    [MinLength(1, ErrorMessage = "Tag must be between 1 and 200 characters.")]
    public string Tag { get; set; }
        
    /// <summary>
    /// The ID of the space for the DeploymentTargetTag
    /// </summary>
    [Required]
    public string SpaceId { get; set; }    
}