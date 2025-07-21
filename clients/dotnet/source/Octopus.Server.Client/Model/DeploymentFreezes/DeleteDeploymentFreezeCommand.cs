using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.DeploymentFreezes;

public class DeleteDeploymentFreezeCommand
{
    [Required] public string Id { get; set; }
}