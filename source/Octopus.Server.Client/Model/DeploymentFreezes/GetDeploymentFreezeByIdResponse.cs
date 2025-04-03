using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Octopus.Server.MessageContracts.Base.Attributes;

namespace Octopus.Client.Model.DeploymentFreezes;

public class GetDeploymentFreezeByIdResponse
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public DateTimeOffset Start { get; set; }

    [Required]
    public DateTimeOffset End { get; set; }
    
    [Optional]
    public Dictionary<string, ReferenceCollection> ProjectEnvironmentScope { get; protected set; }

    [Optional]
    public List<TenantProjectEnvironment> TenantProjectEnvironmentScope { get; protected set; } 
    
    [Optional]
    public string OwnerId { get; set;}

}
