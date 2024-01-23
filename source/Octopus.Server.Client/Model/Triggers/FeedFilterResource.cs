using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers;

public class FeedFilterResource : TriggerFilterResource
{
    public override TriggerFilterType FilterType => TriggerFilterType.FeedFilter;

    [Writeable]
    public List<TriggerPackageReferenceResource> Packages { get; set; } = new();
}

public class TriggerPackageReferenceResource
{
    [Writeable]
    public string DeploymentActionSlug { get; set; }

    [Writeable]
    public string PackageReferenceName { get; set; }
}