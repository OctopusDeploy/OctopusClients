using System;
using System.Collections.Generic;
using Octopus.Client.Model.ScheduledTriggers;

namespace Octopus.Client.Serialization
{
    public class TriggerScheduleConverter : InheritedClassConverter<TriggerScheduleResource, TriggerScheduleType>
    {
        static readonly IDictionary<TriggerScheduleType, Type> ActionTypes =
            new Dictionary<TriggerScheduleType, Type>
            {
                { TriggerScheduleType.CronExpression, typeof (CronTriggerScheduleResource)},
                { TriggerScheduleType.Daily, typeof (DailyTriggerScheduleResource)},
                { TriggerScheduleType.DaysPerWeek, typeof (DaysPerWeekTriggerScheduleResource)},
                { TriggerScheduleType.DaysPerMonth, typeof (DaysPerMonthTriggerScheduleResource)},
            };

        protected override IDictionary<TriggerScheduleType, Type> DerivedTypeMappings => ActionTypes;
        protected override string TypeDesignatingPropertyName => "ScheduleType";
    }
}
