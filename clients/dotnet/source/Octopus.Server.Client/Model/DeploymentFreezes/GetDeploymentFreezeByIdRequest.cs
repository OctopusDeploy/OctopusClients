using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.DeploymentFreezes;

public class GetDeploymentFreezeByIdRequest
{
    [Required]
    public string Id { get; set; }
}
