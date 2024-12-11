using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility;
using Octopus.Server.MessageContracts.Base.Attributes;

namespace Octopus.Client.Model.DeploymentTargetTags;

public class GetDeploymentTargetTagsRequest : IHaveSpaceResource
{
    /// <summary>
    /// The ID of the Space to which the DeploymentTargetTags belong. 
    /// </summary>
    [Required]
    public string SpaceId { get; set; }

    /// <summary>
    /// The DeploymentTargetTag IDs to filter by.
    /// </summary>
    [Optional]
    public IReadOnlyCollection<string> Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The Machine ID to filter by.
    /// </summary>
    [Optional]
    public IReadOnlyCollection<string> MachineIds { get; set; } = Array.Empty<string>();
    
    [Optional]
    public int? Skip { get; set; }
    
    [Optional]
    public int? Take { get; set; }    
}