using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.DeploymentFreezes;

public class CreateDeploymentFreezeResponse
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }
        
    [Required]
    public DateTimeOffset Start { get; set; }
        
    [Required]
    public DateTimeOffset End { get; set; }
        
    [Required]
    public  Dictionary<string, ReferenceCollection> ProjectEnvironmentScope { get; set; }
}