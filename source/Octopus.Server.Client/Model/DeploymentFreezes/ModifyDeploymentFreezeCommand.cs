using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Octopus.Server.MessageContracts.Base.Attributes;

namespace Octopus.Client.Model.DeploymentFreezes;

public class ModifyDeploymentFreezeCommand
{
    [Required] public string Id { get; set; }

    [Required(ErrorMessage = "Please provide a display name.")]
    [MinLength(1)]
    [MaxLength(200)]
    public string Name { get; set; }

    [Required(ErrorMessage = "Please provide a start time.")]
    public DateTimeOffset Start { get; set; }

    [Required(ErrorMessage = "Please provide an end time.")]
    public DateTimeOffset End { get; set; }

    [Optional] public Dictionary<string, ReferenceCollection> ProjectEnvironmentScope { get; set; }
    
    [Optional] public List<TenantProjectEnvironment> TenantProjectEnvironmentScope { get; set; }
    
    [Optional] public string OwnerId { get; set; }
}