using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model.DeploymentTargetTags;

public class GetDeploymentTargetTagByTagRequest : IHaveSpaceResource
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