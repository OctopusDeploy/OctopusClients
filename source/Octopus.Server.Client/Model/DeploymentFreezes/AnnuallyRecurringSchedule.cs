namespace Octopus.Client.Model.DeploymentFreezes
{
    public class AnnuallyRecurringSchedule : RecurringSchedule
    {
        public override RecurringScheduleType Type => RecurringScheduleType.Annually;
    }
}
