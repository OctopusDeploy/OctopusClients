using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Octopus.Server.MessageContracts.Base.Attributes;

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

    [Optional] public Dictionary<string, ReferenceCollection> ProjectEnvironmentScope { get; set; }

    [Optional] public List<TenantProjectEnvironment> TenantProjectEnvironmentScope { get; set; }
}