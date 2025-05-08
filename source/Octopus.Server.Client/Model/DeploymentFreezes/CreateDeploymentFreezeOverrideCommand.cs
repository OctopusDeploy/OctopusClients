using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.DeploymentFreezes;

public class CreateDeploymentFreezeOverrideCommand 
{
    public CreateDeploymentFreezeOverrideCommand(DeploymentResource createDeploymentCommand, IReadOnlyCollection<string> freezeIds, string reason)
    {
        CreateDeploymentCommand = createDeploymentCommand;
        FreezeIds = freezeIds;
        Reason = reason;
    }

    [Required]
    public DeploymentResource CreateDeploymentCommand { get; set; }
    
    [Required]
    public IReadOnlyCollection<string> FreezeIds { get; set; }
    
    [Required]
    public string Reason { get; set; }
}