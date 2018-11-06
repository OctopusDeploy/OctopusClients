using System;
using System.Collections.Generic;
using Octopus.Client.Model.Triggers;
using Octopus.Client.Model.Triggers.ScheduledTriggers;

namespace Octopus.Client.Serialization
{
    public class TriggerFilterConverter : InheritedClassConverter<TriggerFilterResource, TriggerFilterType>
    {
        static readonly IDictionary<TriggerFilterType, Type> FilterTypes =
          new Dictionary<TriggerFilterType, Type>
          {
#pragma warning disable 618
              { TriggerFilterType.MachineFilter, typeof (MachineFilterResource)},
#pragma warning restore 618
              { TriggerFilterType.DeploymentTargetFilter, typeof (DeploymentTargetFilterResource)},
              { TriggerFilterType.DailySchedule, typeof (DailyScheduledTriggerFilterResource)},
              { TriggerFilterType.DaysPerWeekSchedule, typeof (DaysPerWeekScheduledTriggerFilterResource)},
              { TriggerFilterType.DaysPerMonthSchedule, typeof (DaysPerMonthScheduledTriggerFilterResource)},
              { TriggerFilterType.CronExpressionSchedule, typeof (CronScheduledTriggerFilterResource)}
          };

        protected override IDictionary<TriggerFilterType, Type> DerivedTypeMappings => FilterTypes;
        protected override string TypeDesignatingPropertyName => "FilterType";
    }
}