namespace Octopus.Client.Model.DeploymentFreezes
{
    public class DailyRecurringSchedule : RecurringSchedule
    {
        public override RecurringScheduleType Type => RecurringScheduleType.Daily;
    }
}
