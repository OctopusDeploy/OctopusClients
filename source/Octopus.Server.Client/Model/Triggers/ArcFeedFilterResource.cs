using System.Collections.Generic;

namespace Octopus.Client.Model.Triggers;

public class ArcFeedFilterResource : TriggerFilterResource
{
    public List<DeploymentActionPackageResource> Packages { get; set; } = new();
    public override TriggerFilterType FilterType => TriggerFilterType.Arc;
}