using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    public Dictionary<string, ReferenceCollection> ProjectEnvironmentScope { get; set; }
    
    public List<TenantProjectEnvironment> TenantProjectEnvironmentScope { get; set; }
    
    public string OwnerId { get; set; }
    
    public RecurringSchedule RecurringSchedule { get; set; }
}
