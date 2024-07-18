using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.DeploymentTargetTags;

public class DeleteDeploymentTargetTagCommand
{
    /// <summary>
    /// The ID of the space containing the resource(s).
    /// </summary>
    [Required]
    public string SpaceId { get; set; } = null!;
        
    /// <summary>
    /// The Tag of the DeploymentTargetTag to delete.
    /// </summary>
    [Required]
    public string Tag { get; set; }    
}