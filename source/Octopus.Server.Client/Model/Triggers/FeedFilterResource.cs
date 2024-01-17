using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers;

public class FeedFilterResource : TriggerFilterResource
{
    public override TriggerFilterType FilterType => TriggerFilterType.FeedFilter;

    [Writeable]
    public List<FeedFilterReferenceResource> FeedFilterReferences { get; set; } = new();
}

public class FeedFilterReferenceResource
{
    [Writeable]
    public string DeploymentProcessStepSlug { get; set; }
    
    [Writeable]
    public string PackageId { get; set; }
}