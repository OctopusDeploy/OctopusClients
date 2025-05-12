using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.DeploymentTargetTags;

public class GetDeploymentTargetTagsResponse
{
    [Required]
    public IReadOnlyCollection<DeploymentTargetTagResource> DeploymentTargetTags { get; set; }

    [Required]
    public int Count { get; set; }    
}

public class DeploymentTargetTagResource
{
    [Required]
    public string Tag { get; set; }

    [Required]
    public string SpaceId { get; set; }    
}