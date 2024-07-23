using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model.DeploymentTargetTags;

public class DeleteDeploymentTargetTagCommand : IHaveSpaceResource
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