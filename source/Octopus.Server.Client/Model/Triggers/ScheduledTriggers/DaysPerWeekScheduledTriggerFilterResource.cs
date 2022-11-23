using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers;

public class DaysPerWeekScheduledTriggerFilterResource : DailyScheduledTriggerFilterResource
{
    public override TriggerFilterType FilterType => TriggerFilterType.DaysPerWeekSchedule;

    [Writeable]
    public bool Monday { get; set; }

    [Writeable]
    public bool Tuesday { get; set; }

    [Writeable]
    public bool Wednesday { get; set; }

    [Writeable]
    public bool Thursday { get; set; }

    [Writeable]
    public bool Friday { get; set; }

    [Writeable]
    public bool Saturday { get; set; }

    [Writeable]
    public bool Sunday { get; set; }
}