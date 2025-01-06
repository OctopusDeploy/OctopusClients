using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Octopus.Server.MessageContracts.Base.Attributes;

namespace Octopus.Client.Model.DeploymentFreezes;

public class GetDeploymentFreezesRequest
{
    /// <summary>
    /// List of DeploymentFreeze IDs which if specified, filters the result to only include DeploymentFreeze with matching IDs.
    /// </summary>
    [Optional]
    public IReadOnlyCollection<string> Ids { get; set; } = Array.Empty<string>();

    /// <summary>
    /// List of Project IDs which if specified, filters the result to only include DeploymentFreeze with matching Project IDs.
    /// </summary>
    [Optional]
    public IReadOnlyCollection<string> ProjectIds { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// List of Tenant IDs which if specified, filters the result to only include DeploymentFreeze with matching Project IDs.
    /// </summary>
    [Optional]
    public IReadOnlyCollection<string> TenantIds { get; set; } = Array.Empty<string>();

    /// <summary>
    /// List of Environment IDs which if specified, filters the result to only include DeploymentFreeze with matching Environment IDs.
    /// </summary>
    [Optional]
    public IReadOnlyCollection<string> EnvironmentIds { get; set; } = Array.Empty<string>();

    /// <summary>
    /// A partial or complete name to search on. This will perform a "contains" style match against the supplied name or name-fragment
    /// </summary>
    [Optional]
    public string PartialName { get; set; }

    [Required]
    public int Skip { get; set; }

    [Required]
    public int Take { get; set; }
}