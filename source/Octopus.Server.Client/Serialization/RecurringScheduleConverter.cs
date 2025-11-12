using System;
using System.Collections.Generic;
using Octopus.Client.Model.DeploymentFreezes;

namespace Octopus.Client.Serialization
{
    public class RecurringScheduleConverter : InheritedClassConverter<RecurringSchedule, RecurringScheduleType>
    {
        static readonly IDictionary<RecurringScheduleType, Type> RecurringScheduleTypeMappings =
            new Dictionary<RecurringScheduleType, Type>
            {
                { RecurringScheduleType.Daily, typeof(DailyRecurringSchedule) },
                { RecurringScheduleType.Weekly, typeof(WeeklyRecurringSchedule) },
                { RecurringScheduleType.Monthly, typeof(MonthlyRecurringSchedule) },
                { RecurringScheduleType.Annually, typeof(AnnuallyRecurringSchedule) }
            };

        protected override IDictionary<RecurringScheduleType, Type> DerivedTypeMappings => RecurringScheduleTypeMappings;
        protected override string TypeDesignatingPropertyName => "Type";
    }
}
