namespace Octopus.Client.Model.DeploymentFreezes
{
    public enum RecurringScheduleType
    {
        Daily,
        Weekly,
        Monthly,
        Annually
    }

    public enum RecurringScheduleEndType
    {
        Never,
        OnDate,
        AfterOccurrences
    }
}
