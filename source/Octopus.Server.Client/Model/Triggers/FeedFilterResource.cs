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
    public DeploymentActionSlugPackageResource(string deploymentActionSlug, string packageReference)
    {
        DeploymentActionSlug = deploymentActionSlug;
        PackageReference = packageReference;
    }                                                                                                                         

    public string DeploymentActionSlug { get; set; }
    public string PackageReference { get; set; }
}