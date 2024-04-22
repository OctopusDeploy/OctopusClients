using System.Collections.Generic;
using Newtonsoft.Json;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers;

public class FeedFilterResource : TriggerFilterResource
{
    public override TriggerFilterType FilterType => TriggerFilterType.FeedFilter;

    [Writeable]
    public List<DeploymentActionSlugPackageResource> Packages { get; set; } = new();
}

public class DeploymentActionSlugPackageResource
{
    [JsonConstructor]
    public DeploymentActionSlugPackageResource(string DeploymentActionSlug, string PackageReferenceId)
    {
        this.DeploymentActionSlug = DeploymentActionSlug;
        this.PackageReferenceId = PackageReferenceId;
    }                                                                                                                         

    public string DeploymentActionSlug { get; set; }
    public string PackageReferenceId { get; set; }
}