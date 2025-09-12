using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.DeploymentFreezes;

public class GetDeploymentFreezesResponse
{
    [Required] public IReadOnlyCollection<DeploymentFreezeResource> DeploymentFreezes { get; set; }

    [Required] public int Count { get; set; }
}

public class DeploymentFreezeResource
{
    [Required] public string Id { get; set; }

    [Required] public string Name { get; set; }

    [Required] public DateTimeOffset Start { get; set; }

    [Required] public DateTimeOffset End { get; set; }

    public Dictionary<string, ReferenceCollection> ProjectEnvironmentScope { get; set; }

    public List<TenantProjectEnvironment> TenantProjectEnvironmentScope { get; set; }
    
    public string OwnerId { get; set; }
    
    public RecurringSchedule RecurringSchedule { get; set; }
}
