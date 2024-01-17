using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers;

public class CreateReleaseActionResource : TriggerActionResource
{
    public override TriggerActionType ActionType => TriggerActionType.CreateRelease;

    [Writeable]
    public string ChannelId { get; set; }
}