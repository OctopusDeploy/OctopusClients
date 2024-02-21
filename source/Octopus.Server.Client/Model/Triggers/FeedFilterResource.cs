using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers;

public class FeedFilterResource : TriggerFilterResource
{
    public override TriggerFilterType FilterType => TriggerFilterType.FeedFilter;

    [Writeable]
    public List<DeploymentActionPackageResource> Packages { get; set; } = new();

    [Writeable]
    public string FeedCategory { get; set; }
}